using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW.Utils.Files
{
    public class FileSystemHelper
    {
        public static void CreateDirectoryIfNotExists(params string[] folders)
        {
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }

        public static bool TryOpen(string fullPath, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.None).Close(); ;
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(3000);
                }
            }

            return false;
        }

        public static IEnumerable<string> GetFiles(string folder, string filePattern = "*.*", string[] allowedExtensions = null)
        {
            var filesToAdd = Directory.GetFiles(folder, filePattern);

            if (allowedExtensions != null && allowedExtensions.Length > 0)
            {
                filesToAdd = filesToAdd.Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToArray();
            }
            return filesToAdd.OrderBy(Path.GetFileName);
        }
    }
}
