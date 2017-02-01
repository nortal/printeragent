using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using PrinterAgent.DTO;
using PrinterAgent.Util;
using WPFtoolkitFramework.Funciones_y_metodos;


namespace PrinterAgent.Service
{
    public class PrinterAgentService
    {
        private static readonly bool SkipSignatureVerification = bool.Parse(ConfigurationManager.AppSettings["SkipSignatureVerification"]);
        private static readonly string UseDebugPrinter = ConfigurationManager.AppSettings["UseDebugPrinter"];
        private static readonly string TempFilePrefix = ConfigurationManager.AppSettings["TempFilePrefix"];
        private static readonly int AdobeHangSeconds = int.Parse(ConfigurationManager.AppSettings["AdobeHangSeconds"]);

        public string Print(PrintRequestDto request)
        {
            VerifyParameters(request);
            if (!SkipSignatureVerification)
                VerifySignature(request);

            string printerName = GetPrinterName(request.DocumentType);
            
            Logger.LogInfo("Printing using printer: "+ printerName);
            Print(printerName, request.Document);

            return printerName;
        }

        private void VerifyParameters(PrintRequestDto request)
        {
            var missingFields = request.FieldsNullValues();
            if (missingFields.Count > 0)
                throw new Exception("Required parameters are missing: "+string.Join(",",missingFields));
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
                throw e;
            }
            /*
            finally
            {
                File.Delete(fileName);
            }
            */
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
            infoPrintPdf.WindowStyle = ProcessWindowStyle.Hidden;

            var proc = new Process();
            proc.StartInfo= infoPrintPdf;
            proc.Start();
            
            if (!proc.WaitForExit(AdobeHangSeconds*1000))
            {
                proc.CloseMainWindow();
                proc.Kill();
            }
            
    
        }

        private string GetPrinterName(string documentType)
        {
            if (!string.IsNullOrEmpty(UseDebugPrinter))
                return UseDebugPrinter;

            var conf = PrintConfiguration.Instance;
            var printer = conf.Printers?.Find(x => x.DocumentTypes!=null && x.DocumentTypes.Contains(documentType));

            if (printer != null)
                return printer.Name;

            var defaultPrinter = conf.Printers?.Find(x => x.IsDefault!=null && x.IsDefault.Value);
            if (defaultPrinter != null)
                return defaultPrinter.Name;

            throw new ForbiddenException("Neither printer for the given document type nor default printer were found");
            
        }

        private void VerifySignature(PrintRequestDto request)
        {
           var decodedSignature =UrlSafeBase64Converter.ConvertFromBase64Url(request.HashSignature);

           var response = new PrinterConfigurationService().CheckSignature(new SignatureVerificationRequestDto()
            {
                DocumentHash = CreateHash(request.Document, request.HashAlgorithm),
                HashSignature = Convert.ToBase64String(decodedSignature),
                EncryptionAlgorithm = request.SignatureAlgorithm
            });

            if (response?.Verified == true)
                return;
            
            throw new UnathorizedException("Signature verification failed");
        }

        private string CreateHash(byte[] requestDocument, string requestHashAlgorithm)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(requestHashAlgorithm))
            {
                if (hashAlgorithm == null)
                    throw new Exception(requestHashAlgorithm + " is not recognized as a valid hash algorithm");
                var hash = hashAlgorithm.ComputeHash(requestDocument);
                return Convert.ToBase64String(hash);

            }
        }


        public string Ping(PingRequestDto pingRequest)
        {
            return GetPrinterName(pingRequest.DocumentType);
        }
    }
    
}
