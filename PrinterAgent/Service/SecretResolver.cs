using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using PrinterAgent.DTO;
using PrinterAgent.PrintConfigurationSystem.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public static class SecretResolver
    {
        
        public static string GetPrinterAgentId()
        {
            
            if (string.IsNullOrEmpty(CachedPrintConfiguration.Secret))
            {
                CachedPrintConfiguration.Secret = RegistryDataResolver.GetStoredPrinterAgentId();  
            }
            
            return CachedPrintConfiguration.Secret;
        }
        

    }
}
