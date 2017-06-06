using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PrinterAgent.Util;

namespace PrinterAgent.PrintingHandler
{
    public class AdobePrintingHandler : PrintingHandler
    {

        private static readonly int AdobeHangSeconds = int.Parse(ConfigurationManager.AppSettings["AdobeHangSeconds"]);
        private static readonly string TempFilePrefix = ConfigurationManager.AppSettings["TempFilePrefix"];

        public void Print(string printerName, byte[] document)
        {
            var tempPath = Path.GetTempPath();
            var fileName = tempPath + TempFilePrefix + Guid.NewGuid() + ".pdf";
            File.WriteAllBytes(fileName, document);

            try
            {
                PrintWithAdobe(printerName, fileName);
            }
            catch (Exception e)
            {
                RemoveFile(fileName);
                throw e;
            }

        }

        private void PrintWithAdobe(string printerName, string fileName)
        {

            var adobePath = RegistryDataResolver.GetAcrobatPath();

            if (string.IsNullOrEmpty(adobePath))
                throw new Exception("Adobe is not installed");


            ProcessStartInfo infoPrintPdf = new ProcessStartInfo();

            string flagNoSplashScreen = "/s";
            string flagOpenMinimized = "/h";
            string flagNoDFileOpenDialog = "/o";

            var flagPrintFileToPrinter = string.Format("/t \"{0}\" \"{1}\"", fileName, printerName);
            var args = string.Format("{0} {1} {2} {3}", flagNoSplashScreen, flagNoDFileOpenDialog, flagOpenMinimized, flagPrintFileToPrinter);

            infoPrintPdf.FileName = adobePath;
            infoPrintPdf.Arguments = args;

            infoPrintPdf.CreateNoWindow = true;
            infoPrintPdf.UseShellExecute = false;
            infoPrintPdf.ErrorDialog = false;
            infoPrintPdf.RedirectStandardOutput = true;
            infoPrintPdf.RedirectStandardError = true;
            infoPrintPdf.WindowStyle = ProcessWindowStyle.Hidden;

            var proc = new Process();
            proc.StartInfo = infoPrintPdf;
            proc.Start();
            new Thread(() => WaitAdobe(proc, fileName)).Start();

        }

        private void WaitAdobe(Process process, string fileName)
        {
            try
            {
                if (!process.WaitForExit(AdobeHangSeconds * 1000))
                {
                    process.CloseMainWindow();
                    process.Kill();
                }
            }
            catch
            {
            }
            finally
            {
                RemoveFile(fileName);
            }
        }

        private void RemoveFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch
            {

            }
        }
    }
}
