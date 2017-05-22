using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HW.Logging;
using HW.Storages;
using HW.Utils.Files;

namespace HW.ScanService.Scanner.Services
{
    public class Scanner
    {
        private string[] _inputFolders;
        private int _interval;
        private readonly IStorageService _storageService;

        private ILogger _logger = Logger.Current;

        Thread workingThread;
        ManualResetEvent workStop;
        AutoResetEvent newFile;

        public Scanner(string[] inputFolders, int interval, IStorageService storageService)
        {
            _inputFolders = inputFolders;
            _interval = interval;
            _storageService = storageService;

            FileSystemHelper.CreateDirectoryIfNotExists(_inputFolders);

            workStop = new ManualResetEvent(false);
            newFile = new AutoResetEvent(false);
            workingThread = new Thread(Scanning);
        }


        private void Scanning()
        {
            do
            {
                _logger.LogInfo("Scanning... ");

                var files = GetFiles(_inputFolders);

                if (workStop.WaitOne(TimeSpan.Zero))
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
            while (WaitHandle.WaitAny(new WaitHandle[] { workStop, newFile }, _interval) != 0);
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
