using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Utils.Services
{
    public class CollectorServiceProperties : ServiceProperties
    {
        public CollectorServiceProperties(string propsArgs) : base(propsArgs)
        {
        }

        public CollectorServiceProperties(ServiceProperties serviceProperties) : base(serviceProperties)
        {
        }


        public string ProcessLocation => Properties["processLocation"];
        public string OutputLocation => Properties["outputsLocation"];
    }
}
