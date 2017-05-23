using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using HW.Management.Common;

namespace HW.Management
{
    public class BasicManager
    {
        protected readonly ServiceBusClient ServiceBusClient;

        public BasicManager(string serviceUrl)
        {

            ServiceUrl = serviceUrl;

            ServiceBusClient = new ServiceBusClient(serviceUrl);
        }

        public string ServiceUrl { get; private set; }


    }
}
