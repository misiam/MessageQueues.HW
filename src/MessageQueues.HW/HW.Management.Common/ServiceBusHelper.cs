using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Utils.Services;
using Microsoft.ServiceBus;

namespace HW.Management.Common
{
    public static class ServiceBusHelper
    {
        public static void CreateTopicIfNotExists(this NamespaceManager namespaceManager, string topicName)
        {
            if (!namespaceManager.TopicExists(topicName))
            {
                try
                {
                    namespaceManager.CreateTopic(topicName);

                }
                catch (Microsoft.ServiceBus.Messaging.MessagingException e)
                {
                    
                }
            }
        }

        public static void CreateSubscriptionIfNotExists(this NamespaceManager namespaceManager, string topicName, string subsName)
        {
            if (!namespaceManager.SubscriptionExists(topicName, subsName))
            {
                try
                {
                    namespaceManager.CreateSubscription(topicName, subsName);
                }
                catch (Microsoft.ServiceBus.Messaging.MessagingException e)
                {

                }

            }
        }

        public static void CreateQueueIfNotExists(this NamespaceManager namespaceManager, string queueName)
        {
            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }
        }


        //Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace;StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355
        public static string CreateConnectionString(QueueChunkedBaseProperties properties)
        {
            var cnsBuilder = new ServiceBusConnectionStringBuilder();
            cnsBuilder.Endpoints.Add(properties.Endpoint);
            cnsBuilder.RuntimePort = properties.RuntimePort;
            cnsBuilder.ManagementPort = properties.ManagementPort;
            cnsBuilder.StsEndpoints.Add(properties.StsEndpoint);

            string cnString = cnsBuilder.ToString();

            return cnString;
        }

    }
}
