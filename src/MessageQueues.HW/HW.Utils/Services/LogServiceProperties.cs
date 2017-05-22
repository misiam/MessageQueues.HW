using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Utils.Services
{
    public class LogServiceProperties : ServiceProperties
    {
        public LogServiceProperties(string propsArgs) : base(propsArgs)
        {
        }

        public LogServiceProperties(ServiceProperties serviceProperties) : base(serviceProperties)
        {
        }

        public string LogPath => Properties["logPath"];


        public static string GetLogPath(string[] args, string defaultPath)
        {
            const string LOG = "logPath";
            var properties = ServiceProperties.GetProperties(args); 
            if (properties == null)
            {
                return defaultPath;
            }
            var logProperties = new LogServiceProperties(properties);

            return logProperties.LogPath;
        }

    }
}
