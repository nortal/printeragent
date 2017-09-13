using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using PrinterAgent.Util;

namespace PrinterAgent.PrintingHandler
{
    public class GhostScriptPrintingHandler : PrintingHandler
    {
        private static readonly string BitsPerPixel = ConfigurationManager.AppSettings["BitsPerPixel"];

        protected override void Print(string printerName, string filePath)
        {
            byte[] buffer = File.ReadAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\gsdll32.dll");

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
                Logger.LogInfo(string.Join(" ", switches));

                var callback = new CallbackStdIO();
                processor.StartProcessing(switches.ToArray(), callback);

                Logger.LogInfo("GS StdOut:\n" + callback.OutLog);
                if (!string.IsNullOrEmpty(callback.ErrorLog))
                    Logger.LogError("GS StdError:\n" + callback.ErrorLog);
                
                
            }
        }


        private class CallbackStdIO : GhostscriptStdIO
        {
            public string OutLog = "";
            public string ErrorLog = "";
            public CallbackStdIO() : base(false, true, true)
            {
            }

            public override void StdIn(out string input, int count)
            {
                throw new NotImplementedException();
            }

            public override void StdOut(string output)
            {
                OutLog += output+Environment.NewLine;
            }

            public override void StdError(string error)
            {
                ErrorLog += error + Environment.NewLine;
                
            }
        }

    }
}
