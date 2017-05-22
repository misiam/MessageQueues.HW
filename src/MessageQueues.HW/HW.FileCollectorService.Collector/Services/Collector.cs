using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HW.Logging;
using HW.Storages;
using HW.Utils.Files;
using HW.Utils.Services;

namespace HW.FileCollectorService.Collector.Services
{
    public class Collector
    {
        private readonly QueueChunkedStorage _storageService;
        private readonly CollectorServiceProperties _props;
        private int _interval;

        private ILogger _logger = Logger.Current;

        Thread workingThread;
        ManualResetEvent workStop;
        AutoResetEvent newFile;

        public Collector(CollectorServiceProperties props, int interval, QueueChunkedStorage storageService)
        {
            _props = props;
            _interval = interval;
            _storageService = storageService;



            FileSystemHelper.CreateDirectoryIfNotExists(props.ProcessLocation, props.OutputLocation);

            workStop = new ManualResetEvent(false);
            newFile = new AutoResetEvent(false);
            workingThread = new Thread(Scanning);



        }


        private void Scanning()
        {
            _storageService.NewFileAppeared += StorageServiceOnNewFileAppeared;
            //_storageService.OnToDownloadQueueItem();
            do
            {
                _logger.LogInfo("Scanning... ");

                if (workStop.WaitOne(TimeSpan.Zero))
                    return;
                _storageService.GetToDownloadQueueItem();
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { workStop, newFile }, _interval) != 0);
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
            workingThread.Start();
        }

        public void StopScanning()
        {
            workingThread.Abort();
        }
    }
}
