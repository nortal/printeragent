using PrinterAgentServer.Cache;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.Service
{
    public static class SecretResolver
    {
        
        public static string GetPrinterAgentId()
        {
            
            if (string.IsNullOrEmpty(PrintConfigurationCache.Secret))
            {
                PrintConfigurationCache.Secret = RegistryDataResolver.GetStoredPrinterAgentId();  
            }
            
            return PrintConfigurationCache.Secret;
        }
        

    }
}
