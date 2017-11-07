using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
        private static readonly Process printAgentProcess = new Process();
        private static bool _expectedServerKill;

        static ProcessManager(){
            ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"] = RegistryDataResolver.GetPcsUrl();           
            printAgentProcess.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PrinterAgentServer.exe";
            printAgentProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            printAgentProcess.StartInfo.UseShellExecute = true;
            
        }

        public static void Start()
        {
            try
            {
                Thread.Sleep(1000);
                Logger.LogInfo("Main service started");
                while (true)
                {
                    var exitCode = Run();
                    Logger.LogInfo("Exit code received from PrinterAgentServer: " + exitCode);
                    if (exitCode == (int) AgentExceptionCode.AgentDisabled)
                    {
                        Program.SetStatus("Agent has been disabled. Shutting down.");
                    }
                    else if (exitCode == (int) AgentExceptionCode.SpoolerNotRunning)
                    {
                        Program.SetStatus("Spooler is not running. Shutting down.");
                        Environment.Exit(exitCode);
                    }
                    else if (_expectedServerKill)
                    {
                        _expectedServerKill = false;
                        break;
                    }
                    else
                    {
                        Program.SetStatus("Shutting down since unexpected error occured");
                        Environment.Exit(0);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Stop();
            }

        }

        private static int Run()
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

            printAgentProcess.StartInfo.Arguments = $"{CommandSwitch.Port}={port} {CommandSwitch.ParentProcessId}={Process.GetCurrentProcess().Id}";
            printAgentProcess.Start();
            Program.SetStatus("Local server is running on port " + port);
            printAgentProcess.WaitForExit();

            return printAgentProcess.ExitCode;
        }
        
        public static void Stop()
        {
            try
            {
                if (!printAgentProcess.HasExited)
                {

                    _expectedServerKill = true;
                    printAgentProcess.Kill();
                    Logger.LogInfo("Process killed");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}