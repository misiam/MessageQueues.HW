using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Definitions;
using Microsoft.ServiceBus;

namespace HW.Management
{
    public class Manager : BasicManager
    {

        public Manager(string serviceUrl) : base(serviceUrl)
        {
            this.ServiceBusClient.InitTopics();
        }


        public void UpdateProperties(IDictionary<string, string> properties)
        {
            this.ServiceBusClient.UpdateProperties(properties);
    
        }

        public void StartScanning()
        {
            this.ServiceBusClient.SendServiceCommand(ServiceCommandType.Start);
        }
        public void StopScanning()
        {
            this.ServiceBusClient.SendServiceCommand(ServiceCommandType.Stop);
        }
    }
}
