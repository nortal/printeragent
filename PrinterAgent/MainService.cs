using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using PrinterAgent.Service;
using PrinterAgent.Util;

namespace PrinterAgent
{
    public static class MainServiceStarter
    {
        private static MainService MainService;

        public static void Start()
        {
            
            ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"] = RegistryDataResolver.GetPcsUrl();
            SetupSessionSwitchEventHandler();
            MainService = new MainService();
            MainService.Start();
            
        }

        public static void Stop()
        {
            MainService?.Stop();
            
        }

        private static void SetupSessionSwitchEventHandler()
        {
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Logger.LogInfo(e.Reason.ToString());

            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.ConsoleDisconnect:
                    Logger.LogInfo("Processing lock/disconnect");
                    MainService.Stop();
                    MainService = null;
                    break;

                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.ConsoleConnect:
                    Logger.LogInfo("Processing unlock/connect");
                    if (MainService == null) { 
                        MainService = new MainService();
                        MainService.Start();
                    }
                    break;
            }
        }

        public static string GetStatus()
        {
            return MainService?.Status;
        }
    }


    public class MainService
    {
        
        private ConfigurationUpdater confUpdater;
        private SocketServer.SocketServer server;

        private bool isLoggedIn;
        public string Status;

        public void Start()
        {
            isLoggedIn = true;
            Logger.LogInfo("Main service started");

            Status = "Starting configuration updater...";
            confUpdater = new ConfigurationUpdater();

            var confPollerTask = new Task(() => confUpdater.Poll());
            confPollerTask.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            confPollerTask.Start();

            Status = "Getting networking port...";
            PrinterConfigurationService pcsService = new PrinterConfigurationService();
            
            var port = pcsService.GetNetworkingPort();
            while (port == null && isLoggedIn)
            {
                Thread.Sleep(30000);
                port = pcsService.GetNetworkingPort();
            }

            if (!isLoggedIn)
                return;
            
            server = new SocketServer.SocketServer(port.Value);
            var serverTask = new Task(() => server.Start());
            serverTask.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            serverTask.Start();

            Status = "Local server is running on port "+ port;
            
        }
        
        public void Stop()
        {
            isLoggedIn = false;
            confUpdater?.Stop();
            server?.Stop();
            Logger.LogInfo("Main service stopped");
        }


        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Logger.LogErrorToPrintConf(exception?.ToString());
        }
    }


}
