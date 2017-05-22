using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace HW.ScanService
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount.NetworkService,

            };
            var service = new System.ServiceProcess.ServiceInstaller
            {
                ServiceName = "HW.ScanService",
                DisplayName = "HW ScanService",
                StartType = ServiceStartMode.Manual
            };

            Installers.Add(process);
            Installers.Add(service);
        }

    }
}
