using System;
using System.Collections.Generic;


namespace HW.Utils.Services
{
    public class ServiceProperties : BaseProperties
    {
        public ServiceProperties(string propsArgs) : base(propsArgs)
        {
        }

        public ServiceProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public ServiceProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        public int ScanInterval
        {
            get
            {
                int interval;
                if (!Properties.ContainsKey(PropsNames.ScanIntervalInMilliseconds) || !int.TryParse(Properties[PropsNames.ScanIntervalInMilliseconds], out interval))
                {
                    interval = 5 * 1000;
                }
                return interval;
            }
        }

    }
}
