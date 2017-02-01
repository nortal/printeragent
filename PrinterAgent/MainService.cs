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
        
        private ConfigurationPoller confPoller;
        private TempFilesRemover tempFilesRemover;
        private SocketServer.SocketServer server;

        private bool isLoggedIn;
        public string Status;

        public void Start()
        {
            isLoggedIn = true;
            Logger.LogInfo("Main service started");
            Status = "Getting networking port...";
            confPoller = new ConfigurationPoller();
            new Thread(() => confPoller.Poll()).Start();
            //wait until configuration is polled
            Thread.Sleep(3000);

            var port = PrintConfiguration.Instance?.AgentListeningPort;
            while (port == null && isLoggedIn)
            {
                Thread.Sleep(30000);
                port = PrintConfiguration.Instance?.AgentListeningPort;
            }

            if (!isLoggedIn)
                return;
            
            server = new SocketServer.SocketServer(port.Value);
            new Thread(server.Start).Start();

            Status = "Local server started";

            tempFilesRemover = new TempFilesRemover();
            new Thread(tempFilesRemover.Start).Start();

        }
        
        public void Stop()
        {
            isLoggedIn = false;
            confPoller?.Stop();
            server?.Stop();
            tempFilesRemover?.Stop();
            Logger.LogInfo("Main service stopped");
        }

    }


}
