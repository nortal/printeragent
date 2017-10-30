using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using PrinterAgentServer.Service;
using PrinterAgentServer.Util;

namespace PrinterAgentServer
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            int port;
            if (args.Length==0 || !int.TryParse(args[0], out port))
                throw new System.Exception("No port specified");

            int parentProcessId;
            if (args.Length > 1 && int.TryParse(args[1], out parentProcessId))
                new ParentProcessMonitor(parentProcessId).Start();

            ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"] = RegistryDataResolver.GetPcsUrl();

            var confPollerTask = new Task(() => new ConfigurationUpdater().Poll());
            confPollerTask.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            confPollerTask.Start();

            new SocketServer.SocketServer(port).Start();

        }

        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Logger.LogErrorToPrintConf(exception?.ToString());
        }
    }
    
}