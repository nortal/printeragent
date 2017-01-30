using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrinterAgent.SocketServer;
using PrinterAgent.Util;

namespace PrinterAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EnsureOneInstanceIsRunning();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += CloseApp;
            AppDomain.CurrentDomain.ProcessExit += CloseApp;

            var form = new Form1();
            new Thread(MainServiceStarter.Start).Start();
            
            new Thread((f) => UpdateWidgetStatus(form)).Start();
            Application.Run(form);
            
        }

        private static void EnsureOneInstanceIsRunning()
        {
            if (!WinProcessHandler.IsCurrentUserRunningSingleInstance())
            {
                Logger.LogInfo("Shutting down since process instance is already running");
                Environment.Exit(0);
            }
                
        }

        private static void UpdateWidgetStatus(Form1 form)
        {
            while (true)
            {
                Thread.Sleep(3000);
                var status = MainServiceStarter.MainService?.Status;
                if (form.Status != status)
                    form.Status = status;
            }
                
        }
        

        private static void CloseApp(object sender, EventArgs eventArgs)
        {
            Logger.LogInfo("Shutting down");
            MainServiceStarter.Stop();
            Environment.Exit(0);
        }

    }
}
