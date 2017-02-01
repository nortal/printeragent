﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class ConfigurationPoller
    {
        private static readonly int PollingInterval = Int32.Parse(ConfigurationManager.AppSettings["ConfigurationPollingIntervalMinutes"]);

        private bool isRunning;
        readonly PrinterConfigurationService printConfService = new PrinterConfigurationService();
        readonly ConfigurationRegistrationService registrationService = new ConfigurationRegistrationService();
        
        public void Poll()
        {
            isRunning = true;
            while (isRunning)
            {
                try
                {
                    Logger.LogInfo("Configuration sync");
                    var id = registrationService.GetPrinterAgentId();
                    if (!string.IsNullOrEmpty(id))
                    {
                        PrintConfiguration.Instance = printConfService.GetConfiguration(id);
                        UpdateBackendConfIfRequired(id);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogErrorToPrintConf("Configuration poller received exception "+e);
                }

                Thread.Sleep(PollingInterval*60*1000);
                
            }
            


        }

        public void Stop()
        {
            isRunning = false;
        }

        private void UpdateBackendConfIfRequired(string id)
        {
            var backendConf = PrintConfiguration.Instance;
            var localConf = registrationService.CreateLocalConfiguration();
            var backendPrinters = backendConf?.Printers?.Select(x => x.Name).ToList().OrderBy(x=>x).ToList() ?? new List<string>();
            var localPrinters = localConf?.Printers?.Select(x => x.Name).ToList().OrderBy(x => x).ToList() ?? new List<string>();
            if (!backendPrinters.SequenceEqual(localPrinters))
            {
                Logger.LogInfo("Updating backend configuration with the local printers: "+string.Join(",", localPrinters));
                PrintConfiguration.Instance = printConfService.UpdateConfiguration(id, localConf);
            }

        }
    }
}
