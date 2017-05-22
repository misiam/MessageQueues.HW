using System;
using System.Collections.Generic;
using System.Linq;


namespace HW.Utils.Services
{
    public class ServiceProperties
    {
        public ServiceProperties(string propsArgs)
        {
            //inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|logPath=C:\winserv\scanner.log   
            PropsArgs = propsArgs;
            ParseProperties(propsArgs); 
        }

        public ServiceProperties(ServiceProperties serviceProperties)
        {
            Properties = serviceProperties.Properties;
            PropsArgs = serviceProperties.PropsArgs;
        }

        public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();
        public string PropsArgs { get; private set; }


        protected void ParseProperties(string propsArgs)
        {
            var props = PropsArgs.Split('|');
            foreach (var prop in props)
            {
                var splited = prop.Split('=');

                if (splited.Length>=2)
                {
                    Properties.Add(splited[0], splited[1]);
                }
            }
        }

        public static ServiceProperties GetProperties(string[] args)
        {
            string propArgs = args.FirstOrDefault(x => x.StartsWith("-props:"));
            if (propArgs == null)
            {
                return null;
            }
            propArgs = new string(propArgs.Skip("-props:".Length).ToArray());
            var props = new ServiceProperties(propArgs);

            return props;
        }

    }
}
