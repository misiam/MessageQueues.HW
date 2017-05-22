using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Storages;

namespace HW.FileCollectorService.Collector
{
    public interface IFilesHandler
    {
        void Handle(IEnumerable<string> filesToHandle, IFolderStorageService storageService, string path);
    }
}
