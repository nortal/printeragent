using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PrinterAgent.Cache;
using PrinterAgent.DTO;
using PrinterAgent.Model;
using PrinterAgent.PrintingHandler;
using PrinterAgent.Util;


namespace PrinterAgent.Service
{
    public class PrinterAgentService
    {
        private static readonly bool SkipSignatureVerification = bool.Parse(ConfigurationManager.AppSettings["SkipSignatureVerification"]);
        private static readonly int AcceptBatchesTimeoutSeconds = int.Parse(ConfigurationManager.AppSettings["AcceptBatchesTimeoutSeconds"]);


        private readonly PrinterConfigurationService printConfService = new PrinterConfigurationService();

        public string Print(PrintRequestDto request)
        {
            var printId = request.PrintId;
            StoreBatch(request);

            if (request.BatchNr != request.BatchesTotal)
                return null;

            var aggregatedRequest = CreateAggregatedRequest(printId);
            var document = aggregatedRequest.GetDecodedDocument();

            VerifySignature(document, request.HashAlgorithm, request.Signature, request.SignatureAlgorithm);

            string printerName = GetPrinterName(request.DocumentType);

            Logger.LogInfo("Printing using printer: " + printerName);
            new GhostScriptPrintingHandler().Print(printerName, document, printId);

            return printerName;
            
        }

        private void StoreBatch(PrintRequestDto request)
        {
            var isTheFirstRequest = PrintRequestsCache.AddBatch(request);
            if (!isTheFirstRequest)
                return;

            Task.Delay(TimeSpan.FromSeconds(AcceptBatchesTimeoutSeconds*2))
                .ContinueWith(t => PrintRequestsCache.RemoveRequestInProgress(request.PrintId));
            
        }

        private PrintRequest CreateAggregatedRequest(string printId)
        {
            var printRequest = PrintRequestsCache.GetPrintRequestInProgress(printId);
            var startTime = DateTime.Now;

            while (printRequest.HasMissingBatches()) { 
                if ((DateTime.Now - startTime).TotalSeconds >= AcceptBatchesTimeoutSeconds)
                    throw new ForbiddenException("Accept batches timeout");
                Thread.Sleep(100);
            }
            return printRequest;

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

        private void VerifySignature(byte[] document, string hashAlg, byte[] signature, string signAlg)
        {
            if (SkipSignatureVerification)
                return;
            var verification = printConfService.CheckSignature(document, hashAlg, signature, signAlg);
            if (verification!=null && verification.Verified == true)
                return;
            
            throw new UnathorizedException("Signature verification failed");
        }
        

        public string Ping(PingRequestDto pingRequest)
        {
            return GetPrinterName(pingRequest.DocumentType);
        }

        public void CheckDocumentType(CheckDocumentTypeDto pingRequest)
        {
            var printerByDocType = GetPrinterNameByDocType(pingRequest.DocumentType);

            if (string.IsNullOrEmpty(printerByDocType))
                throw new ForbiddenException("There is no printer set for the current document type");
        }
    }
    
}
