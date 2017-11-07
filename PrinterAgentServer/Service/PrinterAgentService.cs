using System.Configuration;
using PrinterAgentServer.DTO;
using PrinterAgentServer.Exception;
using PrinterAgentServer.PrintingHandler;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.Service
{
    public class PrinterAgentService
    {
        private static readonly bool SkipSignatureVerification = bool.Parse(ConfigurationManager.AppSettings["SkipSignatureVerification"]);
        

        private readonly PrinterConfigurationService printConfService = new PrinterConfigurationService();

        public string Print(PrintRequestDto request)
        {
            if (!SkipSignatureVerification)
                VerifySignature(request);

            string printerName = GetPrinterName(request.DocumentType);
            
            Logger.LogInfo("Printing using printer: "+ printerName);
            new GhostScriptPrintingHandler().Print(printerName, request.Document);

            return printerName;
        }
        
        
        private string GetPrinterName(string documentType)
        {
            var printerByDocType = GetPrinterNameByDocType(documentType);

            if (!string.IsNullOrEmpty(printerByDocType))
                return printerByDocType;

            var defaultPrinter = PrinterManager.GetDefaultPrinter();
            if (!string.IsNullOrEmpty(defaultPrinter))
                return defaultPrinter;

            throw new ForbiddenException("Neither printer for the given document type nor default printer were found");
            
        }

        private string GetPrinterNameByDocType(string documentType)
        {
            if (documentType == null)
                return null;
            var conf = printConfService.GetConfiguration();
            var printer = (conf?.DocTypeMappings != null && conf.DocTypeMappings.ContainsKey(documentType)) ? conf.DocTypeMappings[documentType] : null;
            return printer;
        }

        private void VerifySignature(PrintRequestDto request)
        {
           if(printConfService.CheckSignature(request)?.Verified == true)
                return;
            
            throw new UnathorizedException("Signature verification failed");
        }
        

        public void Ping(PingRequestDto pingRequest)
        {
        }

        public void CheckDocumentType(CheckDocumentTypeDto pingRequest)
        {
            var printerByDocType = GetPrinterNameByDocType(pingRequest.DocumentType);

            if (string.IsNullOrEmpty(printerByDocType))
                throw new ForbiddenException("There is no printer set for the current document type");
        }
    }
    
}
