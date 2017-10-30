using PrinterAgentServer.Util;

namespace PrinterAgentServer.Service
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
