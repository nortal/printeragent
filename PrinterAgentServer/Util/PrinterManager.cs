using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Net.Mime;
using PrinterAgentServer.Exception;

namespace PrinterAgentServer.Util
{
    public static class PrinterManager
    {
        public static List<string> GetAvailablePrinters()
        {
            List<string> printers = new List<string>();
            try
            {
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(printer);
                }
                
            }
            catch (Win32Exception)
            {
                Environment.Exit((int) AgentExceptionCode.SpoolerNotRunning);              
            }
            return printers;
        }

        public static string GetDefaultPrinter()
        {
            return new PrinterSettings().PrinterName;
        }
    }
}
