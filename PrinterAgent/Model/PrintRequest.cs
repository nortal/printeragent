using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PrinterAgent.Model
{
    public class PrintRequest
    {
        public PrintRequest(string printId, string documentType, string[] batches)
        {
            PrintId = printId;
            DocumentType = documentType;
            Batches = batches;
        }

        public string PrintId { get; }
        public string DocumentType { get; }
        public string[] Batches { get; }

        public byte[] GetDecodedDocument()
        {
            var batchSize = Batches[0].Length;
            var minCapacity = batchSize*Batches.Length;
            var maxCapacity = minCapacity + batchSize + 2; //+2 for rightpadding

            var base64UrlEncoded = new StringBuilder(minCapacity, maxCapacity);
            
            foreach (var byteArr in Batches)
            {
                base64UrlEncoded.Append(byteArr);
            }

            var base64Encoded = base64UrlEncoded.Replace('-', '+').Replace('_', '/');
            var rightPadding = new string('=', (4 - base64UrlEncoded.Length % 4) % 4);
            base64Encoded.Append(rightPadding);

            var decodedBytes = Convert.FromBase64String(base64Encoded.ToString());

            return decodedBytes;
        }
        public bool HasMissingBatches()
        {
            return Batches.Contains(null);
        }
    
    }
}
