using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghostscript.NET.Processor;

namespace PrinterAgent.PrintingHandler
{
    public class GhostScriptPrintingHandler : PrintingHandler
    {
        protected override void Print(string printerName, string filePath)
        {
            
            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dPrinted");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dSAFER");
                switches.Add("-dNoCancel");
                switches.Add("-sDEVICE=mswinpr2");
                switches.Add("-sOutputFile=%printer%" + printerName);
                switches.Add("-f");
                switches.Add(filePath);

                processor.StartProcessing(switches.ToArray(), null);
            }
        }
        
    }
}
