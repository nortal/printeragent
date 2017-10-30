using System;
using System.IO;
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
            var reqContent = Convert.ToBase64String(Serialize(@"C:\Users\jevgenisa\Desktop\test.pdf"));

            var content = "<img src=\"http://localhost:56789/api/print-jobs/dummy.png?"+
                "document=" + reqContent+
                "&signature=c2lnbmF0dXJlDQo=" +
                 "&document-type=SINGLE_BARCODE" +
                "&timestamp="+DateTime.Now.Ticks+
                "&signature-algorithm=alg1"+
                "&hash-algorithm=SHA1" +
                "\" />";

            File.WriteAllText(htmlFilePath, content);
            
        }

        public static byte[] Serialize(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }
        
    }
}
