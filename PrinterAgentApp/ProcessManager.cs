using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using PrinterAgentServer;
using PrinterAgentServer.Exception;
using PrinterAgentServer.Service;
using PrinterAgentServer.SocketServer;
using PrinterAgentServer.Util;

namespace PrinterAgentApp
{
    public static class ProcessManager
    {
        private static readonly Process PrintAgentProcess = new Process();

        private static readonly int[] NonFatalExceptions = {(int)AgentExceptionCode.AgentDisabled};

        static ProcessManager(){
            ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"] = RegistryDataResolver.GetPcsUrl();           
            PrintAgentProcess.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PrinterAgentServer.exe";
            PrintAgentProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            PrintAgentProcess.StartInfo.UseShellExecute = true;
            
        }

        public static void Start()
        {
            try
            {
                Thread.Sleep(1000);
                Logger.LogInfo("Main service started");

                int exitCode;
                do
                {
                    exitCode = Run();
                    Logger.LogInfo("Exit code received from PrinterAgentServer: " + exitCode);
                    switch (exitCode)
                    {
                        case (int)AgentExceptionCode.AgentDisabled:
                            Program.SetStatus("Agent has been disabled. Shutting down.");
                            break;
                        case (int)AgentExceptionCode.SpoolerNotRunning:
                            Program.SetStatus("Spooler is not running. Shutting down.");
                            break;
                        default:
                            Program.SetStatus("Shutting down since unexpected error occured");
                            break;
                    }
                    
                } while (NonFatalExceptions.Contains(exitCode));

                Environment.Exit(exitCode);

            }
            catch (ThreadInterruptedException)
            {
                Stop();
            }

        }

        private static int Run()
        {
            var port = GetNetworkingPort();

            PrintAgentProcess.StartInfo.Arguments = $"{CommandSwitch.Port}={port} {CommandSwitch.ParentProcessId}={Process.GetCurrentProcess().Id}";
            PrintAgentProcess.Start();
            Program.SetStatus("Local server is running on port " + port);
            PrintAgentProcess.WaitForExit();

            return PrintAgentProcess.ExitCode;
        }

        private static int? GetNetworkingPort()
        {
            PrinterConfigurationService pcsService = new PrinterConfigurationService();

            var port = pcsService.GetNetworkingPort();
            if (port == null)
            {
                Program.SetStatus("Getting networking port...");
                do
                {
                    Thread.Sleep(30000);
                    port = pcsService.GetNetworkingPort();
                } while (port == null);
            }

            return port;
        }


        private static void Stop()
        {
            try
            {
                Logger.LogInfo("Killing server process");
                PrintAgentProcess.Kill();
                Logger.LogInfo("Server process killed");
            }
            catch (InvalidOperationException e)
            {
                Logger.LogInfo(e.ToString());
            }
        }
    }
}