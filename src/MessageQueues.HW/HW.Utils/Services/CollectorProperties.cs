using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Utils.Services
{
    public class CollectorProperties : BaseProperties
    {
        public CollectorProperties(string propsArgs) : base(propsArgs)
        {
        }

        public CollectorProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public CollectorProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        public string ProcessLocation => Properties[PropsNames.ProcessLocation];
        public string OutputLocation => Properties[PropsNames.OutputsLocation];
        public ServiceProperties ServiceProperties => new ServiceProperties(Properties);
    }
}
