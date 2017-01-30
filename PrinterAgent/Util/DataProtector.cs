using System;
using System.Security.Cryptography;
namespace PrinterAgent.Util
{
    public class DataProtector
    {
        static byte[] s_aditionalEntropy = { 9, 8, 1, 7, 5 };
        public static byte[] Protect(byte[] data)
        {
            try
            {
                return ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                Logger.LogError(e.ToString());
                return null;
            }
        }

        public static byte[] Unprotect(byte[] data)
        {
            try
            {
                return ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                Logger.LogError(e.ToString());
                return null;
            }
        }
        
    }
}
