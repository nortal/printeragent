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
            int port = 0;
            int parentProcessId = 0;

            foreach (var a in args)
            {
                var arg=a.Split('=');
                if (arg.Length!=2)
                    continue;
                var paramName = arg[0];
                var paramValue = arg[1];
                if (paramName.Equals(CommandSwitch.Port, StringComparison.InvariantCultureIgnoreCase))
                    int.TryParse(paramValue, out port);
                else if (paramName.Equals(CommandSwitch.ParentProcessId, StringComparison.InvariantCultureIgnoreCase))
                    int.TryParse(paramValue, out parentProcessId);
            }

            
            if (port <= 0)
                throw new System.Exception("No valid port specified");
                        
            if (parentProcessId > 0)
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