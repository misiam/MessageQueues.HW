using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Definitions;
using HW.Utils.Services;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace HW.Management.Common
{
    public class ServiceBusClient
    {
        private readonly string _serviceUrl;
        private NamespaceManager _namespaceManager;

        public event EventHandler<string> PropsSubscription;

        public ServiceBusClient(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _namespaceManager = NamespaceManager.CreateFromConnectionString(_serviceUrl);
        }


        public void InitTopics()
        {
            _namespaceManager.CreateTopicIfNotExists(Topics.BROADCAST_PROPERTIES_TOPIC);
            _namespaceManager.CreateSubscriptionIfNotExists(Topics.BROADCAST_PROPERTIES_TOPIC, Topics.BROADCAST_PROPERTIES_SUBS_COLLECTOR);
            _namespaceManager.CreateSubscriptionIfNotExists(Topics.BROADCAST_PROPERTIES_TOPIC, Topics.BROADCAST_PROPERTIES_SUBS_SCANNER);

            _namespaceManager.CreateTopicIfNotExists(Topics.RUN_SERVICE_COMMAND_TOPIC);
            _namespaceManager.CreateSubscriptionIfNotExists(Topics.RUN_SERVICE_COMMAND_TOPIC, Topics.RUN_SERVICE_COMMAND_SUBS_COLLECTOR);
            _namespaceManager.CreateSubscriptionIfNotExists(Topics.RUN_SERVICE_COMMAND_TOPIC, Topics.RUN_SERVICE_COMMAND_SUBS_SCANNER);


        }


        public void UpdateProperties(IDictionary<string, string> properties)
        {
            var serviceProperties = new BaseProperties(properties);

            var client = TopicClient.CreateFromConnectionString(_serviceUrl, Topics.BROADCAST_PROPERTIES_TOPIC);
            var message = new BrokeredMessage(serviceProperties.ToString());

            client.Send(message);
        }

        public void SendServiceCommand(ServiceCommandType commandType)
        {
            var client = TopicClient.CreateFromConnectionString(_serviceUrl, Topics.RUN_SERVICE_COMMAND_TOPIC);
            var message = new BrokeredMessage(commandType);
            client.Send(message);
        }



        public void OnPropsUpdated(string subscriptionName, Action<string> propsUpdated)
        {

            var client = SubscriptionClient.CreateFromConnectionString(_serviceUrl, Topics.BROADCAST_PROPERTIES_TOPIC, subscriptionName,  ReceiveMode.PeekLock);
            client.OnMessage(message =>
            {
                try
                {
                    propsUpdated(message.GetBody<string>());
                    message.Complete();
                }
                catch (Exception e)
                {
                    message.Abandon();
                    throw;
                }
            });
        }

        public void OnSendServiceCommand(string subscriptionName, Action<ServiceCommandType> propsUpdated)
        {

            var client = SubscriptionClient.CreateFromConnectionString(_serviceUrl, Topics.RUN_SERVICE_COMMAND_TOPIC, subscriptionName, ReceiveMode.ReceiveAndDelete);
            client.OnMessage(message =>
            {

                try
                {
                    propsUpdated(message.GetBody<ServiceCommandType>());
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }



    }
}
