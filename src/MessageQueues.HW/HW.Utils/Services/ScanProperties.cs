using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Utils.Services
{
    public class ScanProperties : BaseProperties
    {
        public ScanProperties(string propsArgs) : base(propsArgs)
        {
        }

        public ScanProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public ScanProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        public ServiceProperties ServiceProperties => new ServiceProperties(Properties);

        public string[] InputLocations => Properties[PropsNames.InputFolders].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

    }
}
