using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HW.Definitions;
using HW.Logging;
using HW.Management.Common;
using HW.Storages;
using HW.Utils.Files;
using HW.Utils.Services;

namespace HW.FileCollectorService.Collector.Services
{
    public class Collector
    {
        private readonly QueueChunkedStorage _storageService;
        private CollectorProperties _props;

        private ILogger _logger = Logger.Current;

        Thread scanningThread;
        AutoResetEvent scanStop;
        private ServiceBusClient _serviceBusClient;

        public Collector(CollectorProperties props)
        {
            _props = props;
            //_interval = interval;

            var queueChunckedProps = new QueueChunkedBaseProperties(props);
            _storageService = new QueueChunkedStorage(queueChunckedProps);

            FileSystemHelper.CreateDirectoryIfNotExists(props.ProcessLocation, props.OutputLocation);

            scanStop = new AutoResetEvent(false);
            scanningThread = new Thread(Scanning);

            string serviceUrl = ServiceBusHelper.CreateConnectionString(queueChunckedProps);
            _serviceBusClient = new ServiceBusClient(serviceUrl);
            _serviceBusClient.InitTopics();
            _serviceBusClient.OnPropsUpdated(Topics.BROADCAST_PROPERTIES_SUBS_COLLECTOR, OnPropsSubscription);
            _serviceBusClient.OnSendServiceCommand(Topics.RUN_SERVICE_COMMAND_SUBS_COLLECTOR, OnRunServiceCommand);
        }



        private void Scanning()
        {
            _storageService.NewFileAppeared += StorageServiceOnNewFileAppeared;

            //_storageService.OnToDownloadQueueItem();
            //_serviceBusClient.PropsSubscription += ServiceBusClientOnPropsSubscription;
            do
            {
                _logger.LogInfo("Scanning... ");

                if (scanStop.WaitOne(_props.ServiceProperties.ScanInterval))
                    return;
                _storageService.GetToDownloadQueueItem();
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { scanStop /*, newFile*/ }, _props.ServiceProperties.ScanInterval) != 0);
        }

        private void OnPropsSubscription(string propsAgrs)
        {
            _logger.LogInfo("OnPropsSubscription: "  + propsAgrs);
            try
            {
                var props = new CollectorProperties(propsAgrs);
                _props.Update(props);
                FileSystemHelper.CreateDirectoryIfNotExists(_props.ProcessLocation, _props.OutputLocation);
                

            }
            catch (Exception e)
            {
                _logger.LogError("OnPropsSubscription: " + e);
                throw;
            }
        }
        private void OnRunServiceCommand(ServiceCommandType commandType)
        {
            switch (commandType)
            {
                case ServiceCommandType.Start:
                    {
                        if (scanningThread == null || !scanningThread.IsAlive)
                        {
                            scanningThread = new Thread(Scanning);
                            scanStop = new AutoResetEvent(false);
                            StartScan();
                            _logger.LogInfo("Scanning started");
                        }
                        else
                        {
                            _logger.LogInfo("Scanning already started");
                        }
                        break;
                    }
                case ServiceCommandType.Stop:
                    {
                        scanStop.Set();
                        scanningThread.Join(_props.ServiceProperties.ScanInterval);
                        _logger.LogInfo("Scanning stopped");
                        break;
                    }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }



        private void StorageServiceOnNewFileAppeared(object sender, string fileName, Stream stream)
        {
            string processLocation = _props.ProcessLocation;
            var localFolderStorage = new LocalFolderStorage(processLocation);

            string newFileName = DateTime.UtcNow.ToFileTimeUtc() + fileName;
            string fileForProcess = localFolderStorage.SaveToStorage(stream, newFileName);

            try
            {
                string scanBarcode = BarcodeScanner.GetBarcodeIfExists(fileForProcess);
                if (!string.IsNullOrWhiteSpace(scanBarcode))
                {
                    File.Delete(fileForProcess);
                    IFolderStorageService folderStorageService = new LocalFolderStorage(_props.OutputLocation);

                    IFilesHandler imagesHandler = new PdfAggregatorFilesHandler();

                    string aggregatedFileName = $"{scanBarcode}_{DateTime.UtcNow.ToFileTimeUtc()}.pdf";
                    _logger.LogInfo("GetFiles from: " + processLocation);
                    var files = FileSystemHelper.GetFiles(processLocation, allowedExtensions: new[] { ".png", ".jpg", ".jpeg", ".bmp" });
                    imagesHandler.Handle(files, folderStorageService, aggregatedFileName);

                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                throw;
            }
        }


        public void StartScan()
        {
            scanningThread.Start();
        }

        public void StopScanning()
        {
            scanningThread.Abort();
        }
    }
}
