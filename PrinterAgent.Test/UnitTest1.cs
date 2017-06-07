using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrinterAgent.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void CreateHtml()
        {
            
            //var reqContent = Convert.ToBase64String(bytes);
            //var reqContent = Convert.ToBase64String(Serialize(@"C: \Users\jevgenisa\Desktop\Learning Material\Java\Java study material\ch08.pdf"));
            var htmlFilePath = @"C:\Users\jevgenisa\Desktop\printagentimage.html";
            var reqContent = Convert.ToBase64String(Serialize(@"C:\Users\jevgenisa\Desktop\pdf.pdf"));

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
