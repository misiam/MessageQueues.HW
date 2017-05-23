using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;

namespace HW.Management.Common
{
    public class Topics
    {
        #region TOPICS
        public const string BROADCAST_PROPERTIES_TOPIC = "BROADCAST_PROPERTIES";
        public const string RUN_SERVICE_COMMAND_TOPIC = "RUN_SERVICE_COMMAND";



        #endregion TOPICS

        #region SUBSCRIPTIONS
        public const string SERVICE_NAME_COLLECTOR = "COLLECTOR";
        public const string SERVICE_NAME_SCANNER = "SCANNER";

        public const string BROADCAST_PROPERTIES_SUBS_ALL = "BROADCAST_PROPERTIES_ALL";
        public const string BROADCAST_PROPERTIES_SUBS_COLLECTOR = BROADCAST_PROPERTIES_TOPIC+ "_" + SERVICE_NAME_COLLECTOR;
        public const string BROADCAST_PROPERTIES_SUBS_SCANNER = BROADCAST_PROPERTIES_TOPIC+ "_" + SERVICE_NAME_SCANNER;

        public const string RUN_SERVICE_COMMAND_SUBS_COLLECTOR = RUN_SERVICE_COMMAND_TOPIC + "_" + SERVICE_NAME_COLLECTOR;
        public const string RUN_SERVICE_COMMAND_SUBS_SCANNER = RUN_SERVICE_COMMAND_TOPIC + "_" + SERVICE_NAME_SCANNER;



        #endregion SUBSCRIPTIONS



    }
}
