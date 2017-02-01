using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using PrinterAgent.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class ConfigurationRegistrationService
    {
        private PrinterConfigurationService confService = new PrinterConfigurationService();

        public string GetPrinterAgentId()
        {
            
            var id = RegistryDataResolver.GetStoredPrinterAgentId();
            if (!string.IsNullOrEmpty(id))
                return id;
            Logger.LogInfo("Agent id does not exist in registry. Registering the computer on a backend service.");
            try
            {
                return Register();
            }
            catch (Exception e)
            {
                return null;
            }
                
        }

        public PrintConfigurationDto CreateLocalConfiguration()
        {
            var request = new PrintConfigurationDto();
            request.Printers = new List<PrinterDto>();
            request.Name = Environment.MachineName;
            request.AdditionalInfo = GetLocalIpAddress();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                request.Printers.Add(new PrinterDto() { Name = printer });
            }
            return request;
        }

        private string GetLocalIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch
            { }
            return null;
        }

        private string Register()
        {
            
            var id = RegisterComputer();
            StoreComputerId(id);
            return id;
        }
        

        private void StoreComputerId(string id)
        {
            RegistryDataResolver.StorePrinterAgentId(id);
        }

        private string RegisterComputer()
        {
            var response = confService.CreateConfiguration(CreateLocalConfiguration());
            return response?.Secret;
        }


    }
}
