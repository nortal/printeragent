using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using PrinterAgent.DTO;
using PrinterAgent.Util;


namespace PrinterAgent.Service
{
    public class PrinterAgentService
    {
        private static readonly bool SkipSignatureVerification = bool.Parse(ConfigurationManager.AppSettings["SkipSignatureVerification"]);
        private static readonly string TempFilePrefix = ConfigurationManager.AppSettings["TempFilePrefix"];
        private static readonly int AdobeHangSeconds = int.Parse(ConfigurationManager.AppSettings["AdobeHangSeconds"]);

        private readonly PrinterConfigurationService printConfService = new PrinterConfigurationService();

        public string Print(PrintRequestDto request)
        {
            if (!SkipSignatureVerification)
                VerifySignature(request);

            string printerName = GetPrinterName(request.DocumentType);
            
            Logger.LogInfo("Printing using printer: "+ printerName);
            Print(printerName, request.Document);

            return printerName;
        }
        
        private void Print(string printerName, byte[] document)
        {
            var tempPath = Path.GetTempPath();
            var fileName = tempPath+ TempFilePrefix+Guid.NewGuid()+".pdf";
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
            proc.StartInfo= infoPrintPdf;
            proc.Start();
            new Thread(() => WaitAdobe(proc, fileName)).Start();
            
        }

        private void WaitAdobe(Process process, string fileName)
        {
            try
            {
                if (!process.WaitForExit(AdobeHangSeconds*1000))
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
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
        private string GetPrinterName(string documentType)
        {
            var conf = printConfService.GetConfiguration();
            var printer = (conf?.DocTypeMappings!=null && conf.DocTypeMappings.ContainsKey(documentType)) ? conf.DocTypeMappings[documentType] : null;

            if (!string.IsNullOrEmpty(printer))
                return printer;

            var defaultPrinter = PrinterManager.GetDefaultPrinter();
            if (!string.IsNullOrEmpty(defaultPrinter))
                return defaultPrinter;

            throw new ForbiddenException("Neither printer for the given document type nor default printer were found");
            
        }
        
        private void VerifySignature(PrintRequestDto request)
        {
           if(printConfService.CheckSignature(request)?.Verified == true)
                return;
            
            throw new UnathorizedException("Signature verification failed");
        }
        

        public string Ping(PingRequestDto pingRequest)
        {
            return GetPrinterName(pingRequest.DocumentType);
        }
    }
    
}
