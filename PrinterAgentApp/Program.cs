using System;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using PrinterAgentServer.Util;

namespace PrinterAgentApp
{
    static class Program
    {
        private static Form _form;
        private static Thread _appThread;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += CloseApp;
            AppDomain.CurrentDomain.ProcessExit += CloseApp;

            _form = new Form();
            _form.Shown += (_,__) =>
            {
                EnsureUserHasEventViewerRights();
                EnsureOneInstanceIsRunning();
                StartAppThread();
            };

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            Application.Run(_form);
        }
        
        private static void EnsureUserHasEventViewerRights()
        {
            if (!Logger.UserHasRights())
            {
                SetStatus("Current user does not have rights for event viewer");
                Environment.Exit(0);
            }
        }
        private static void EnsureOneInstanceIsRunning()
        {
            if (!WinProcessHandler.IsCurrentUserRunningSingleInstance())
            {
                var message = "Shutting down since process instance is already running";
                Logger.LogInfo(message);
                SetStatus(message);
                Environment.Exit(0);
            }
                
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Logger.LogInfo(e.Reason.ToString());

            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.ConsoleDisconnect:
                    Logger.LogInfo("Processing lock/disconnect");
                    StopAppThread();
                    break;

                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.ConsoleConnect:
                    Logger.LogInfo("Processing unlock/connect");
                    StartAppThread();
                    break;
            }
        }

        private static void StartAppThread()
        {
            if (_appThread==null || !_appThread.IsAlive)
            {
                _appThread = new Thread(ProcessManager.Start);
                _appThread.Start();
            }
        }

        private static void StopAppThread()
        {
            if (_appThread!=null && _appThread.IsAlive)
            {
                _appThread.Interrupt();
            }
        }

        public static void SetStatus(string status)
        {
            _form.Status = status;                  
        }
        

        public static void CloseApp(object sender, EventArgs eventArgs)
        {
            CloseApp();
        }

        public static void CloseApp()
        {
            Logger.LogInfo("Shutting down");
            StopAppThread();
            Environment.Exit(0);
        }
    }
}
