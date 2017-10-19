﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using PrinterAgent.Cache;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class ConfigurationUpdater
    {
        private static readonly int PollingInterval = Int32.Parse(ConfigurationManager.AppSettings["ConfigurationPollingIntervalMinutes"]);

        private bool isRunning;
        readonly PrinterConfigurationService printConfService = new PrinterConfigurationService();
        
        public void Poll()
        {
            isRunning = true;
            while (isRunning)
            {
                
                UpdateBackendConfIfRequired();
                Thread.Sleep(PollingInterval*60*1000);
              
            }
            


        }

        public void Stop()
        {
            isRunning = false;
        }

        private void UpdateBackendConfIfRequired()
        {
            var lastSentPrinters = PrintConfigurationCache.LastSentPrinters!=null ? PrintConfigurationCache.LastSentPrinters.OrderBy(x => x).ToList() : null;

            var localPrinters = PrinterManager.GetAvailablePrinters().OrderBy(x => x).ToList();

            if (lastSentPrinters==null || !lastSentPrinters.SequenceEqual(localPrinters))
            {
                Logger.LogInfo("Updating backend configuration with the local printers: "+string.Join(",", localPrinters));
                printConfService.UpdateConfiguration();
            }

        }
    }
}
