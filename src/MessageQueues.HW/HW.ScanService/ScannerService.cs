using System;
using System.ServiceProcess;
using HW.Storages;
using HW.Utils.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using ILogger = HW.Logging.ILogger;

namespace HW.ScanService
{
    public class ScannerService : ServiceBase
    {
        private readonly Scanner.Services.Scanner _scanner;
        private static ILogger _logger;

        public ScannerService(BaseProperties props)
        {

            var logger = HW.Logging.Logger.Current;
            logger.LogInfo("PropsArgs:");
            logger.LogInfo(props.PropsArgs);

            foreach (var property in props.Properties)
            {
                logger.LogInfo(property.Key + "|" +property.Value);
            }

            //var folders = props.Properties["inputFolders"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            logger.LogInfo("scanInterval");
            //int interval;
            //if (!props.Properties.ContainsKey("scanInterval") || !int.TryParse(props.Properties["scanInterval"], out interval))
            //{
            //    interval = 5 * 1000;
            //}

            //IStorageService storageService = GetStorage(props);
            var scanProperties = new ScanProperties(props);
            _scanner = new Scanner.Services.Scanner(scanProperties);

        }

        private static void Main(string[] args)
        {

            var logFactory = GetLogFactory(args, @"C:\winserv\scanner.log");

            var logger = HW.Logging.Logger.Current;
            logger.SetActualLogger(logFactory.GetLogger("HW.ScanService"));

            logger.LogInfo("Main");
            foreach (var arg in args)
            {
                logger.LogInfo(arg);
            }


            var props = BaseProperties.GetProperties(args);
            if (args.Length > 0 && args[0].Equals("console"))
            {
                var serv = new ScannerService(props);
                serv.StartScanning();

                Console.ReadKey();
                serv.StopScanning();
            }
            else //windows service
            {
                try
                {
                    Run(new ScannerService(props));
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
                StartScanning();
            }
            catch (Exception e)
            {

                if (_logger != null)
                {
                    _logger.LogError(e);
                }
            }

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

        protected override void OnStop()
        {
            StopScanning();
        }


        public void StartScanning()
        {
            _scanner.StartScan();
        }
        public void StopScanning()
        {
            _scanner.StopScanning();
        }

        private static LogFactory GetLogFactory(string[] args, string defaultPath)
        {

            string logPath = LogBaseProperties.GetLogPath(args, defaultPath);
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
