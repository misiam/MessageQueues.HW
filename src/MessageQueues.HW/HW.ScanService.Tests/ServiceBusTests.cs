using System;
using System.Linq;
using HW.Management.Common;
using HW.Storages;
using HW.Utils.Services;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW.ScanService.Tests
{
    [TestClass]
    public class ServiceBusTests
    {
        private const string COMMAND_LINE_ARGS = @"-props:inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|"
        + "Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace|StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace|RuntimePort=9354|ManagementPort=9355";


        public NamespaceManager NamespaceManager { get; set; }
        public string ConnectionString { get; set; }

        [TestInitialize]
        public void Init()
        {
            ConnectionString = GetServiseBusConnectionString(COMMAND_LINE_ARGS);
            NamespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);
        }

        [TestMethod]
        public void ConnectToServiseBus()
        {
            Console.WriteLine(NamespaceManager.GetQueues().Count());

            var queueClient = QueueClient.CreateFromConnectionString(ConnectionString, "MyQueue");

        }

        [TestMethod]
        public void ShowQueuesTopicsAndSubscriptions()
        {

            Console.WriteLine("-- Queues --");
            foreach (var queue in NamespaceManager.GetQueues())
            {
                Console.WriteLine(queue.Path);
            }

            Console.WriteLine("-- Topics --");
            foreach (var topic in NamespaceManager.GetTopics())
            {
                Console.WriteLine(topic.Path);
                foreach (var subscription in NamespaceManager.GetSubscriptions(topic.Path))
                {
                    Console.WriteLine("\t{0}", subscription.Name);
                }
            }
        }

        [TestMethod]
        public void CleanUpQueues()
        {
            foreach (var queue in NamespaceManager.GetQueues())
            {
                NamespaceManager.DeleteQueue(queue.Path);
                Console.WriteLine("Queue deleted: " + queue.Path);
            }
        }

        [TestMethod]
        public void CleanUpTopics()
        {
            foreach (var topic in NamespaceManager.GetTopics())
            {
                NamespaceManager.DeleteTopic(topic.Path);
                Console.WriteLine("topic deleted: " + topic.Path);
            }
        }

        [TestMethod]
        public void SendFileChunksTest()
        {
            var properties = new QueueChunkedBaseProperties(COMMAND_LINE_ARGS);
            var queueChunkedStorage = new QueueChunkedStorage(properties);

            queueChunkedStorage.SaveToStorage(@"..\..\..\Samples\Img__001.PNG");
        }


        [TestMethod]
        public void GetToDownloadQueue()
        {
            var properties = new QueueChunkedBaseProperties(COMMAND_LINE_ARGS);
            var queueChunkedStorage = new QueueChunkedStorage(properties);
            queueChunkedStorage.GetToDownloadQueueItem();

        }


        [TestMethod]
        public void OnToDownloadQueue()
        {
            var properties = new QueueChunkedBaseProperties(COMMAND_LINE_ARGS);
            var queueChunkedStorage = new QueueChunkedStorage(properties);
            queueChunkedStorage.OnToDownloadQueueItem();

        }

        private string GetServiseBusConnectionString(string commandLineArgs)
        {
            var properties = new QueueChunkedBaseProperties(commandLineArgs);
            string cnString = ServiceBusHelper.CreateConnectionString(properties);


            return cnString;
        }
    }
}
