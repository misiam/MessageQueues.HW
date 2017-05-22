using System;

namespace HW.Utils.Services
{
    public class QueueChunkedServiceProperties : ServiceProperties
    {
        public QueueChunkedServiceProperties(ServiceProperties serviceProperties) : base(serviceProperties)
        {
        }

        public QueueChunkedServiceProperties(string propsArgs) : base(propsArgs)
        {
        }

        //Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace;StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355
        public Uri Endpoint => new Uri(Properties["Endpoint"]);
        public Uri StsEndpoint => new Uri(Properties["StsEndpoint"]);
        public int RuntimePort => int.Parse(Properties["RuntimePort"]);
        public int ManagementPort => int.Parse(Properties["ManagementPort"]);

    }
}
