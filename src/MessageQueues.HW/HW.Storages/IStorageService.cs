using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Utils.Services;

namespace HW.Storages
{
    public interface IStorageService
    {
        void SaveToStorage(string fileName);
    } 
}
