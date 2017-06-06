using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgent.PrintingHandler
{
    public class GhostScriptPrintingHandler : PrintingHandler
    {
        protected override void Print(string printerName, string filePath)
        {
            filePath = @"C:\Users\jevgenisa\Desktop\eHL\pritneragent\dontworry.pdf";

            printerName = @"\\tarp\3 floor corridor printer";

            var psInfo = new ProcessStartInfo();
            psInfo.Arguments =
                $" -dPrinted -dBATCH -dNoCancel -dNOPAUSE -sDEVICE=mswinpr2 -sOutputFile=\"%printer%{printerName}\" \"{filePath}\"";
            psInfo.FileName = @"C:\Program Files\gs\gs9.10\bin\gswin64c.exe";
            psInfo.UseShellExecute = false;

            using (var process = Process.Start(psInfo))
            {
                //TODO: try different ms
                process.WaitForExit(20000);
            }
        }
        
    }
}
