using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghostscript.NET.Processor;
using PrinterAgent.Util;

namespace PrinterAgent.PrintingHandler
{
    public class GhostScriptPrintingHandler : PrintingHandler
    {
        private static readonly string BitsPerPixel = ConfigurationManager.AppSettings["BitsPerPixel"];

        protected override void Print(string printerName, string filePath)
        {
            byte[] buffer = File.ReadAllBytes("gsdll32.dll");

            using (GhostscriptProcessor processor = new GhostscriptProcessor(buffer))
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dPrinted");
                switches.Add("-dBATCH");
                switches.Add("-dSAFER");
                switches.Add("-dNoCancel");
                switches.Add("-dNOPAUSE");
                if (!string.IsNullOrEmpty(BitsPerPixel))
                    switches.Add("-dBitsPerPixel="+ BitsPerPixel);

                switches.Add("-sDEVICE=mswinpr2");
                switches.Add("-sOutputFile=%printer%" + printerName);
                switches.Add("-f");
                switches.Add(filePath);

                Logger.LogInfo(string.Concat(switches," "));
                processor.StartProcessing(switches.ToArray(), null);
            }
        }
        
    }
}
