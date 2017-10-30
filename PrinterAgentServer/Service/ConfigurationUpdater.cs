using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.Service
{
    public class ConfigurationUpdater
    {
        private static readonly int PollingInterval = Int32.Parse(ConfigurationManager.AppSettings["ConfigurationPollingIntervalMinutes"]);
        
        readonly PrinterConfigurationService _printConfService = new PrinterConfigurationService();
        
        public void Poll()
        {
            while (true)
            {                
                UpdateBackendConfIfRequired();
                Thread.Sleep(PollingInterval*60*1000);              
            }            
        }
        
        private void UpdateBackendConfIfRequired()
        {
            var lastSentPrinters = CachedPrintConfiguration.LastSentPrinters!=null ? CachedPrintConfiguration.LastSentPrinters.OrderBy(x => x).ToList() : null;

            var localPrinters = PrinterManager.GetAvailablePrinters().OrderBy(x => x).ToList();

            if (lastSentPrinters==null || !lastSentPrinters.SequenceEqual(localPrinters))
            {
                Logger.LogInfo("Updating backend configuration with the local printers: "+string.Join(",", localPrinters));
                _printConfService.UpdateConfiguration();
            }

        }
    }
}
