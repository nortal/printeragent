using System;

namespace PrinterAgent.Util
{
    static class UrlSafeBase64Converter
    {
        public static byte[] ConvertFromBase64Url(string input)
        {
            string incoming = input.Replace('_', '/').Replace('-', '+');
            switch (input.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);
            //string originalText = Encoding.ASCII.GetString(bytes);
            return bytes;

        }

        public static string ConvertToBase64Url(byte[] bytes)
        {
            char[] padding = { '=' };
            string base64String= Convert.ToBase64String(bytes).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
            return base64String;

        }
    }
}
