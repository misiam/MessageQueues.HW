using System;
using System.Collections.Generic;
using System.IO;
using HW.Utils.Services;


namespace HW.Storages
{
    public class LocalFolderStorage : IFolderStorageService
    {
        private string _outputLocation;

        public LocalFolderStorage(string outputLocation)
        {
            this._outputLocation = outputLocation;
        }

        public void SaveToStorage(string fileName)
        {
            File.Move(fileName, Path.Combine(_outputLocation, Path.GetFileName(fileName)));
        }

        public string SaveToStorage(Stream stream, string fileName)
        {
            fileName = Path.Combine(_outputLocation, fileName);
            using (var fileStream = File.Create(fileName))
            {
                stream.CopyTo(fileStream);
                fileStream.Close();
            }

            return fileName;
        }
    }
}
