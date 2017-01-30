using PrinterAgent.DTO;

namespace PrinterAgent.Util
{
    public class PrintConfiguration
    {
        private static PrintConfigurationDto instance = null;
        private static readonly object padlock = new object();


        public static PrintConfigurationDto Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance;
                }

            }

            set
            {
                lock (padlock)
                {
                   instance = value;
                }
            }
        }


    }
}
