using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrinterAgent.Test
{
    [TestClass]
    public class HtmlGenerator
    {

        [TestMethod]
        public void CreateHtml()
        {
                        
            var htmlFilePath = @"C:\Users\jevgenisa\Desktop\printagentimage.html";
            var base64 = Convert.ToBase64String(Serialize(@"C:\Users\jevgenisa\Desktop\Oracle Certified Professional Java SE 7 Programmer Exams 1Z0-804 and 1Z0-805 A Comprehensive OCPJP 7 Certification Guide.pdf"));
            var batchSize = 10000;
            var batchesTotal = (int) Math.Ceiling(base64.Length / (float) batchSize);
            var printId = Guid.NewGuid();
            var htmlContent = "";
            for (int i = 0; i< batchesTotal; i++)
            {
                var batch = new string(base64.Take(i * batchSize + batchSize).Skip(i * batchSize).ToArray());

                htmlContent += "<img src=\"http://localhost:56789/api/print-jobs/dummy.png?";
                htmlContent += "document=" + batch;                
                htmlContent += "&";
                htmlContent += "document-type=SINGLE_BARCODE";
                htmlContent += "&";
                htmlContent += "timestamp="+ DateTime.Now.Ticks;
                htmlContent += "&";
                htmlContent += "batchNr=" + (i+1);
                htmlContent += "&";
                htmlContent += "batchesTotal=" + batchesTotal;
                htmlContent += "&";
                htmlContent += "printId=" + printId;
                if (i + 1 == batchesTotal)
                {
                    htmlContent += "&";
                    htmlContent += "signature=c2lnbmF0dXJlDQo=";
                    htmlContent += "&";
                    htmlContent += "signature-algorithm=alg1";
                    htmlContent += "&";
                    htmlContent += "hash-algorithm=SHA1";
                }
                htmlContent += "\" />"+Environment.NewLine;
            }
            
            File.WriteAllText(htmlFilePath, htmlContent);
            
        }

        public static byte[] Serialize(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }
        
    }
}
