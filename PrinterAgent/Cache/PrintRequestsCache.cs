using System.Collections.Generic;
using PrinterAgent.DTO;
using PrinterAgent.Model;

namespace PrinterAgent.Cache
{
    public class PrintRequestsCache
    {
        private static readonly List<PrintRequest> PrintRequestsInProgress = new List<PrintRequest>();
        private static readonly object Padlock1 = new object();

        public static void AddBatch(PrintRequestDto requestDto)
        {
            lock (Padlock1)
            {
                var request = PrintRequestsInProgress.Find(r => r.PrintId == requestDto.PrintId);
                if (request == null)
                {
                    request = new PrintRequest(requestDto.PrintId, requestDto.DocumentType, new string[requestDto.BatchesTotal]);
                    PrintRequestsInProgress.Add(request);
                }
                request.Batches[requestDto.BatchNr-1] = requestDto.Document;
            }
        }

        public static PrintRequest GetPrintRequestInProgress(string printId)
        {
            return PrintRequestsInProgress.Find(r => r.PrintId == printId);                
         
        }

    }
}
