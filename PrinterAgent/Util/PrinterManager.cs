using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgent.Util
{
    public static class PrinterManager
    {
        public static List<string> GetAvailablePrinters()
        {
            List<string> printers = new List<string>();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers;
        }

        public static string GetDefaultPrinter()
        {
            return new PrinterSettings().PrinterName;
        }
    }
}
