using System;
using System.Collections.Generic;

namespace HW.Utils.Services
{
    public class QueueChunkedBaseProperties : BaseProperties
    {
        public QueueChunkedBaseProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public QueueChunkedBaseProperties(string propsArgs) : base(propsArgs)
        {
        }

        public QueueChunkedBaseProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        //Endpoint=sb://epbygrow0257t3.grodno.epam.com/ServiceBusDefaultNamespace;StsEndpoint=https://epbygrow0257t3.grodno.epam.com:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355
        public Uri Endpoint => new Uri(Properties[PropsNames.Endpoint]);
        public Uri StsEndpoint => new Uri(Properties[PropsNames.StsEndpoint]);
        public int RuntimePort => int.Parse(Properties[PropsNames.RuntimePort]);
        public int ManagementPort => int.Parse(Properties[PropsNames.ManagementPort]);


    }
}
