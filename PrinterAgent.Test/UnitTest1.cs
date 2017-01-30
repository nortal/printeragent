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
            var bytes = Serialize(@"C:\Users\jevgenisa\Desktop\dontworry1.pdf");
            var reqContent = Convert.ToBase64String(bytes);
            //var reqContent = Serialize(@"C: \Users\jevgenisa\Desktop\Learning Material\Java\Java study material\OCP Java SE 7 Programmer II Certification Guide.pdf");
            var htmlFilePath = @"C: \Users\jevgenisa\Desktop\printagentimage.html";
            //var reqContent = Convert.ToBase64String(Serialize(@"C:\Users\jevgenisa\Desktop\mb2-710.pdf"));
            
            var content = "<img src=\"http://localhost:8888/api/print-jobs/dummy.gif?document=" + reqContent+ 
                "&signature=qwerty" +
                 "&document-type=REFERRAL" +
                "&timestamp="+DateTime.Now.Ticks+
                "&signature-algorithm=alg1"+
                "&hash-algorithm=SHA-256" +
                "\" />";

            File.WriteAllText(htmlFilePath, content);
            
        }

        public static byte[] Serialize(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }
        
    }
}
