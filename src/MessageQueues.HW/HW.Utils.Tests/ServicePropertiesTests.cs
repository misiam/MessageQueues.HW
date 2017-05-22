using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HW.Utils.Services;

namespace HW.Utils.Tests
{
    [TestClass]
    public class ServicePropertiesTests
    {
        [TestMethod]
        public void TryCreateTestProperties()
        {
            const string inputPart = @"C:\winserv\inputs\1;C:\winserv\inputs\2";
            const string scanInterval = @"5000";
            const string logPath = @"C:\winserv\scanner.log";


            string propArgs =
                $"inputFolders={inputPart}|scanInterval={scanInterval}|logPath={logPath}";
            ServiceProperties props = new ServiceProperties(propArgs);

            Assert.IsTrue(props.Properties["inputFolders"] == inputPart);
            Assert.IsTrue(props.Properties["scanInterval"] == scanInterval);
            Assert.IsTrue(props.Properties["logPath"] == logPath);

        }

        [TestMethod]
        public void ParseLogPath()
        {
            string logProp = @"C:\winserv\scanner.log";
            string[] args = new[] { @"-props:inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|logPath=" + logProp };
            string logPath = LogServiceProperties.GetLogPath(args, logProp);

            Assert.IsTrue(logProp == logPath);
        }

        [TestMethod]
        public void ParseQueueConnectionProps()
        {
            string logProp = @"C:\winserv\scanner.log";
            string commandLine =
                @"-props:inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|logPath=" + logProp;


            string endpoint = "sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace";
            string runtimePort = "9354";
            string managementPort = "9355";

            commandLine +=
                $"|Endpoint={endpoint}|RuntimePort={runtimePort}|ManagementPort={managementPort}";
            string[] args = new[] {commandLine};

            var serviceProperties = new ServiceProperties(commandLine);
            



            IList<KeyValuePair<string,string>> values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Endpoint", endpoint),
                new KeyValuePair<string, string>("RuntimePort", runtimePort),
                new KeyValuePair<string, string>("ManagementPort", managementPort),
            };

            foreach (var pair in values)
            {
                var props = serviceProperties.Properties;
                Assert.IsTrue(props.ContainsKey(pair.Key) && props[pair.Key] == pair.Value);
            }
            
        }



    }
}
