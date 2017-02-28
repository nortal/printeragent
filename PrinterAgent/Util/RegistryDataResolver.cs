using System;
using System.Text;
using Microsoft.Win32;

namespace PrinterAgent.Util
{
    static class RegistryDataResolver
    {
        private const string PrinterAgentIdPath = @"SOFTWARE\Nortal Print Agent";
        private const string PrinterAgentIdKey = @"PrinterAgentId";

        public static string GetAcrobatPath()
        {
            string adobePath1 = null;
            try
            {
                adobePath1 = (string)Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe").GetValue("");
            }
            catch (Exception e)
            {
                // ignored
            }

            if (!string.IsNullOrEmpty(adobePath1))
                return adobePath1;

            string adobePath2 = null;
            try
            {
                adobePath2 = (string)Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths\Acrobat.exe").GetValue("");
            }
            catch (Exception e)
            {
                // ignored
            }
            return adobePath2;

        }

        public static string GetStoredPrinterAgentId()
        {
            try
            {
                using (var subKey = Registry.LocalMachine.OpenSubKey(PrinterAgentIdPath))
                {
                    if (subKey == null)
                        return null;
                    var storedId = (byte[]) subKey.GetValue(PrinterAgentIdKey);
                    if (storedId == null)
                        return null;
                    var decryptedId = Encoding.UTF8.GetString(DataProtector.Unprotect(storedId));
                    return decryptedId;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return null;
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
        
    }
}
