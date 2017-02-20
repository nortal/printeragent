using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using PrinterAgent.Service;
using PrinterAgent.Util;

namespace PrinterAgent
{
    public static class MainServiceStarter
    {
        public static MainService MainService;

        public static void Start()
        {
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
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                Logger.LogInfo("Processing lock");
                MainService.Stop();
                
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                Logger.LogInfo("Processing unlock");
                MainService = new MainService();
                MainService.Start();
            }
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
            new Thread(() => confUpdater.Poll()).Start();

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
            new Thread(server.Start).Start();

            Status = "Local server started";

        }
        
        public void Stop()
        {
            isLoggedIn = false;
            confUpdater?.Stop();
            server?.Stop();
            Logger.LogInfo("Main service stopped");
        }

    }


}
