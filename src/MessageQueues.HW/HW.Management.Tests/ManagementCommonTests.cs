using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Management.Common;
using Microsoft.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW.Management.Tests
{
    [TestClass]
    public class ManagementCommonTests
    {

        string serviceUrl = "Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace;StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355";


        [TestMethod]
        public void TestInitTopicsForManagement()
        {
            ServiceBusClient serviceBusClient = new ServiceBusClient(serviceUrl);

            serviceBusClient.InitTopics();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(serviceUrl);


            Assert.IsTrue(namespaceManager.TopicExists(Topics.BROADCAST_PROPERTIES_TOPIC));

            Assert.IsTrue(namespaceManager.SubscriptionExists(Topics.BROADCAST_PROPERTIES_TOPIC, Topics.BROADCAST_PROPERTIES_SUBS_COLLECTOR));
            Assert.IsTrue(namespaceManager.SubscriptionExists(Topics.BROADCAST_PROPERTIES_TOPIC, Topics.BROADCAST_PROPERTIES_SUBS_SCANNER));
        }
    }
}
