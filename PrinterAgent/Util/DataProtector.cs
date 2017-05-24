using System;
using System.Security.Cryptography;
namespace PrinterAgent.Util
{
    public class DataProtector
    {
        static byte[] s_aditionalEntropy = { 9, 8, 1, 7, 5 };
        public static byte[] Protect(byte[] data)
        {
            return ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
        }

        public static byte[] Unprotect(byte[] data)
        {
            
            return ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            
        }
        
    }
}
