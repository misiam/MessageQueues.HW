using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Utils.Services
{
    public class LogBaseProperties : BaseProperties
    {

        public LogBaseProperties(string propsArgs) : base(propsArgs)
        {
        }

        public LogBaseProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public LogBaseProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        public string LogPath => Properties[PropsNames.LogPath];


        public static string GetLogPath(string[] args, string defaultPath)
        {
            var properties = BaseProperties.GetProperties(args); 
            if (properties == null)
            {
                return defaultPath;
            }
            var logProperties = new LogBaseProperties(properties);

            return logProperties.LogPath;
        }


    }
}
