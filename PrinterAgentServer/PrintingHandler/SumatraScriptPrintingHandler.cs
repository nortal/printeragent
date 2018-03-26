using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.PrintingHandler
{
    public class SumatraPrintingHandler : PrintingHandler
    {
        private static readonly string SumatraCommand = ConfigurationManager.AppSettings["SumatraCommand"];

        protected override void Print(string printerName, string filePath)
        {
            try
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        FileName = "SumatraPDF.exe",
                        CreateNoWindow = true,
                        Arguments = SumatraCommand.Replace("{printerName}", printerName).Replace("{fileName}", filePath)
                    }   
                };
                proc.Start();
                proc.WaitForExit();
            }
            catch (System.Exception ex)
            {
                Logger.LogError("Sumatra error:\n" + ex);
                throw;
            }
            
        
        }

    }
}
