using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.FileCollectorService.Collector.Services;
using HW.Storages;
using HW.Utils.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW.CollectorService.Tests
{
    [TestClass]
    public class PdfAggregatorFilesHandlerTests
    {
        private string _processLocation = @"C:\winserv\processLocation\";
        private string _outputLocation = @"C:\winserv\outputs\";

        IList<string> _files = new List<string>();

        [TestInitialize]
        public void Init()
        {
            FileSystemHelper.CreateDirectoryIfNotExists(_processLocation, _outputLocation);
            var sampleFiles = new List<string>() { "Img__001.PNG", "Img__002.PNG" };


            foreach (var fileName in sampleFiles)
            {
                string newFile = _processLocation + fileName;

                string sampleFile = @"..\..\..\..\..\Samples\Sample1\" + fileName;
                if(!File.Exists(newFile))
                    System.IO.File.Copy(sampleFile, newFile);
                _files.Add(newFile);
            }
        }


        [TestCleanup]
        public void RemoveInit()
        {
            foreach (var file in _files)
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        public void CreatePdf()
        {


            var filesHandler = new PdfAggregatorFilesHandler();


            IFolderStorageService storageService = new LocalFolderStorage(_outputLocation);


            string pdfFileName = "test.pdf";

            filesHandler.Handle(_files, storageService, pdfFileName);

            Assert.IsTrue(System.IO.File.Exists(_outputLocation + pdfFileName));

            File.Delete(_outputLocation + pdfFileName);

        }

    }
}
