using System;
using System.ServiceProcess;
using HW.Storages;
using HW.Utils.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using ILogger = HW.Logging.ILogger;

namespace HW.FileCollectorService
{
    public class CollectorService  : ServiceBase
    {
        private readonly Collector.Services.Collector _collector;

        private static ILogger _logger;

        public CollectorService(ServiceProperties props)
        {
            var logger = HW.Logging.Logger.Current;
            _logger = logger;
            logger.LogInfo("PropsArgs1:");
            logger.LogInfo("PropsArgs2:");
            logger.LogInfo(props.PropsArgs);


            logger.LogInfo("props.Properties:");
            foreach (var property in props.Properties)
            {
                logger.LogInfo(property.Key + "|" + property.Value);
            }


            logger.LogInfo("scanInterval");
            int interval;
            if (!props.Properties.ContainsKey("scanInterval") || !int.TryParse(props.Properties["scanInterval"], out interval))
            {
                interval = 5 * 1000;
            }


            QueueChunkedStorage storageService = GetQueueChunkedStorage(new QueueChunkedServiceProperties(props));
            _collector = new Collector.Services.Collector(new CollectorServiceProperties(props), interval, storageService);

        }


        private static void Main(string[] args)
        {

            var logFactory = GetLogFactory(args, @"C:\winserv\collector.log");

            var logger = HW.Logging.Logger.Current;
            logger.SetActualLogger(logFactory.GetLogger("HW.Collector"));

            logger.LogInfo("Main");
            foreach (var arg in args)
            {
                logger.LogInfo(arg);
            }


            var props = ServiceProperties.GetProperties(args);
            if (args.Length > 0 && args[0].Equals("console"))
            {
                var serv = new CollectorService(props);
                serv.StartCollecting();

                Console.ReadKey();
                serv.StopCollecting();
            }
            else //windows service
            {
                try
                {

                    
                    Run(new CollectorService(props));
                }
                catch (Exception e)
                {

                    if (_logger != null)
                    {
                        _logger.LogError(e);
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            try
            {
                StartCollecting();
            }
            catch (Exception e)
            {

                if (_logger != null)
                {
                    _logger.LogError(e);
                }
            }

        }


        protected override void OnStop()
        {
            StopCollecting();
        }
        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_logger != null)
            {
                string senderPart = (sender ?? "[null]").ToString();
                string exceptionPart = e.ToString();
                _logger.LogError("sender: " + senderPart + " | exception: " + exceptionPart);
            }
        }

        private QueueChunkedStorage GetQueueChunkedStorage(QueueChunkedServiceProperties props)
        {
            return new QueueChunkedStorage(props);
        }


        public void StartCollecting()
        {
            _collector.StartScan();
        }
        public void StopCollecting()
        {
            _collector.StopScanning();
        }


        private static LogFactory GetLogFactory(string[] args, string defaultPath)
        {

            string logPath = LogServiceProperties.GetLogPath(args, defaultPath);
            var logConfig = new LoggingConfiguration();

            var target = new FileTarget()
            {
                Name = "Def",
                FileName = logPath,
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };

            logConfig.AddTarget(target);
            logConfig.AddRuleForAllLevels(target);

            var consoleTarget = new ConsoleTarget
            {
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}",
                Name = "console"
            };


            logConfig.AddTarget(consoleTarget);
            logConfig.AddRuleForAllLevels(consoleTarget);

            var logFactory = new LogFactory(logConfig);

            return logFactory;
        }
    }
}
