using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HW.Definitions;
using HW.Logging;
using HW.Management.Common;
using HW.Storages;
using HW.Utils.Files;
using HW.Utils.Services;

namespace HW.ScanService.Scanner.Services
{
    public class Scanner
    {
        private readonly IStorageService _storageService;
        private ILogger _logger = Logger.Current;

        Thread scanningThread;
        AutoResetEvent scanStop;
        private ScanProperties _props;
        private ServiceBusClient _serviceBusClient;

        public Scanner(ScanProperties props)
        {
            var queueChunkedBaseProperties = new QueueChunkedBaseProperties(props);
            _storageService = GetStorage(queueChunkedBaseProperties);
            _props = props;

            FileSystemHelper.CreateDirectoryIfNotExists(props.InputLocations);

            scanStop = new AutoResetEvent(false);
            scanningThread = new Thread(Scanning);


            string serviceUrl = ServiceBusHelper.CreateConnectionString(queueChunkedBaseProperties);
            _serviceBusClient = new ServiceBusClient(serviceUrl);
            _serviceBusClient.InitTopics();
            _serviceBusClient.OnPropsUpdated(Topics.BROADCAST_PROPERTIES_SUBS_SCANNER, OnPropsSubscription);
            _serviceBusClient.OnSendServiceCommand(Topics.RUN_SERVICE_COMMAND_SUBS_SCANNER, OnRunServiceCommand);
        }


        private void Scanning()
        {
            do
            {
                _logger.LogInfo("Scanning... ");


                var files = GetFiles(_props.InputLocations);

                if (scanStop.WaitOne(TimeSpan.Zero))
                    return;

                foreach (var fileInfo in files)
                {
                    string path = fileInfo.FullName;
                    _logger.LogInfo($"       Processing  {path}");

                    if (FileSystemHelper.TryOpen(path, tryCount: 3))
                    {
                        this._storageService.SaveToStorage(path);
                        File.Delete(path);
                    }
                    else
                    {
                        _logger.LogError("Cannot access file " + path);
                    }

                }

            }
            while (WaitHandle.WaitAny(new WaitHandle[] { scanStop }, _props.ServiceProperties.ScanInterval) != 0);
        }
        private IEnumerable<FileInfo> GetFiles(string[] inputFolders)
        {
            var files = new List<FileInfo>();

            foreach (var folder in inputFolders)
            {
                _logger.LogInfo("GetFiles from: " + folder);

                var filesToAdd = FileSystemHelper.GetFiles(folder, "Img_*.*",  new[] { ".png", ".jpg", ".jpeg", ".bmp" });
                files.AddRange(filesToAdd.Select(path=> new FileInfo(path)));
            }
            return files.OrderBy(f=> f.CreationTime);
        }

        private IStorageService GetStorage(QueueChunkedBaseProperties props)
        {
            return new QueueChunkedStorage(props);
        }

        private void OnPropsSubscription(string propsAgrs)
        {

            _logger.LogInfo("OnPropsSubscription: " + propsAgrs);
            try
            {
                var props = new ScanProperties(propsAgrs);
                _props.Update(props);
                FileSystemHelper.CreateDirectoryIfNotExists(_props.InputLocations);

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
