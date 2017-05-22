﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HW.Definitions;
using HW.Logging;
using HW.Utils.Services;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace HW.Storages
{
    public class QueueChunkedStorage : IStorageService
    {
        private readonly QueueChunkedServiceProperties _properties;
        private ILogger _logger;
        const string ToDownloadQueueName = "to_download_queue";

        public delegate void NewFileHandler(object obj, string fileName, Stream stream);
        public event NewFileHandler NewFileAppeared;

        public QueueChunkedStorage(QueueChunkedServiceProperties properties  )
        {
            _logger = Logger.Current;
            _properties = properties;
        }

        private void CreateQueueIfNotExists(NamespaceManager namespaceManager)
        {
            if (!namespaceManager.QueueExists(ToDownloadQueueName))
            {
                _logger.LogInfo("CreateQueue " + ToDownloadQueueName);
                namespaceManager.CreateQueue(ToDownloadQueueName);
            }

        }

        public void SaveToStorage(string filePath)
        {
            _logger.LogInfo("SaveToStorage ");

            var cnString = QueueChunkedStorage.CreateConnectionString(_properties);

            var namespaceManager = NamespaceManager.CreateFromConnectionString(cnString);

            CreateQueueIfNotExists(namespaceManager);

            var fileQueue = $"file_{Guid.NewGuid()}";
            _logger.LogInfo("CreateQueue " + fileQueue);
            namespaceManager.CreateQueue(fileQueue);
            

            try
            {
                _logger.LogInfo("try upload file \""+ filePath + " \" chunks to " + fileQueue);

                using (var fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int chunkSize = 40000;

                    byte[] buffer = new byte[chunkSize>0 ? chunkSize : fsSource.Length];

                    var chunk = new ScanServiceFileChunk();
                    var fileQueueClient = QueueClient.CreateFromConnectionString(cnString, fileQueue);

                    long total = fsSource.Length;
                    chunkSize = chunkSize > total ? (int)total : chunkSize;
                    while (fsSource.Read(buffer, 0, chunkSize) > 0)
                    {

                        _logger.LogInfo($"file \"{filePath}\"");

                        chunk.Data = buffer;
                        var fileChunkMessage = new BrokeredMessage(chunk);
                        fileQueueClient.Send(fileChunkMessage);
                        total -= chunkSize;
                        chunkSize = chunkSize > total ? (int)total : chunkSize;
                        buffer = new byte[chunkSize];
                    }

                }

                _logger.LogInfo("file \"" + filePath + " \" uploaded to queue " + fileQueue);

                var toDownloadQueueClient = QueueClient.CreateFromConnectionString(cnString, ToDownloadQueueName);
                var toDownloadMessage = new BrokeredMessage(new ToDownloadQueueItem
                {
                    FileName = Path.GetFileName(filePath),
                    ToDownloadQueue = fileQueue,
                });
                toDownloadQueueClient.Send(toDownloadMessage);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
                _logger.LogInfo("remove Queue " + fileQueue);
                namespaceManager.DeleteQueue(fileQueue);
                _logger.LogInfo($"Queue {fileQueue} removed ");

            }
        }

        public void GetFromQueue(ToDownloadQueueItem toDownloadQueue)
        {
            _logger.LogInfo($"   GetFromQueue {toDownloadQueue.ToDownloadQueue} file {toDownloadQueue.FileName}");

            var cnString = QueueChunkedStorage.CreateConnectionString(_properties);
            var queueClient = QueueClient.CreateFromConnectionString(cnString, toDownloadQueue.ToDownloadQueue, ReceiveMode.ReceiveAndDelete);


            using (var stream = new MemoryStream())
            {

                while (true)
                {
                    var message = queueClient.Receive();
                    if (message == null)
                    {
                        break;
                    }
                    var fileChunk = message.GetBody<ScanServiceFileChunk>();
                    if (fileChunk == null)
                    {
                        break;
                    }
                    stream.Write(fileChunk.Data, 0, fileChunk.Data.Length);
                }
                stream.Seek(0, SeekOrigin.Begin);

                NewFileAppeared?.Invoke(this, toDownloadQueue.FileName,  stream);
            }

            var namespaceManager = NamespaceManager.CreateFromConnectionString(cnString);
            namespaceManager.DeleteQueue(toDownloadQueue.ToDownloadQueue);
        }

        public void GetToDownloadQueueItem()
        {
            var cnString = QueueChunkedStorage.CreateConnectionString(_properties);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(cnString);

            CreateQueueIfNotExists(namespaceManager);
            
            var toDownloadQueueClient = QueueClient.CreateFromConnectionString(cnString, ToDownloadQueueName, ReceiveMode.ReceiveAndDelete);

            BrokeredMessage message;
            do
            {
                
                message = toDownloadQueueClient.Receive();
                if (message != null)
                {
                    _logger.LogInfo("Get new file from \"to_download_queue\"");
                    GetFromQueue(message.GetBody<ToDownloadQueueItem>());
                }
            } while (message != null);


        }

        public void OnToDownloadQueueItem()
        {
            var cnString = QueueChunkedStorage.CreateConnectionString(_properties);

            var toDownloadQueueClient = QueueClient.CreateFromConnectionString(cnString, ToDownloadQueueName, ReceiveMode.ReceiveAndDelete);
            toDownloadQueueClient.OnMessage(message =>
            {
                if (message != null)
                {
                    GetFromQueue(message.GetBody<ToDownloadQueueItem>());
                }
            });
        }

        //Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace;StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355
        public static string CreateConnectionString(QueueChunkedServiceProperties properties)
        {
            var cnsBuilder = new ServiceBusConnectionStringBuilder();
            cnsBuilder.Endpoints.Add(properties.Endpoint);
            cnsBuilder.RuntimePort = properties.RuntimePort;
            cnsBuilder.ManagementPort = properties.ManagementPort;
            cnsBuilder.StsEndpoints.Add(properties.StsEndpoint);

            string cnString = cnsBuilder.ToString();

            return cnString;
        }

    }
}
