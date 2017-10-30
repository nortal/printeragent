using System.Collections.Generic;

namespace PrinterAgentServer.Util
{
    public class CachedPrintConfiguration
    {
        private static List<string> lastSentPrinters;
        private static readonly object padlock1 = new object();

        private static string secret;
        private static readonly object padlock2 = new object();


        public static List<string> LastSentPrinters
        {
            get
            {
                lock (padlock1)
                {
                    return lastSentPrinters;
                }

            }

            set
            {
                lock (padlock1)
                {
                    lastSentPrinters = value;
                }
            }
        }

        public static string Secret
        {
            get
            {
                lock (padlock2)
                {
                    return secret;
                }

            }

            set
            {
                lock (padlock2)
                {
                    secret = value;
                }
            }
        }

    }
}
