using System.Text;
using Microsoft.Win32;

namespace PrinterAgentServer.Util
{
    public static class RegistryDataResolver
    {
        private const string PrinterAgentIdPath = @"SOFTWARE\Nortal Print Agent";
        private const string PrinterAgentIdKey = @"PrinterAgentId";
        private const string PcsUrlKey = @"PcsUrl";
        private const string Version = @"Version";


        public static string GetPcsUrl()
        {
            var url = GetRegistryValue(PcsUrlKey);
            if (url[url.Length - 1] != '/')
                url += '/';
            return url;
        }

        public static string GetAgentVersion()
        {
            return GetRegistryValue(Version);
        }

        private static string GetRegistryValue(string value)
        {
            using (var subKey = Registry.LocalMachine.OpenSubKey(PrinterAgentIdPath))
            {
                var result = (string) subKey.GetValue(value);                
                return result;
            }
                
        }

        public static string GetStoredPrinterAgentId()
        {
            
            using (var subKey = Registry.LocalMachine.OpenSubKey(PrinterAgentIdPath))
            {
                    
                var storedId = (byte[]) subKey.GetValue(PrinterAgentIdKey);
                if (storedId == null)
                    return null;
                var decryptedId = Encoding.UTF8.GetString(DataProtector.Unprotect(storedId));
                return decryptedId;
            }
        }

        public static void StorePrinterAgentId(string id)
        {
            using (var subKey = Registry.LocalMachine.CreateSubKey(PrinterAgentIdPath))
            {
                var encryptedId = DataProtector.Protect(Encoding.UTF8.GetBytes(id));
                subKey.SetValue(PrinterAgentIdKey, encryptedId);
            }
        }

        public static void CheckWriteAccess()
        {
            using (Registry.LocalMachine.OpenSubKey(PrinterAgentIdPath, true))
            {
                
            }

        }
    }
}
