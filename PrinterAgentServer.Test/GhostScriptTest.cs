using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrinterAgentServer.PrintingHandler;

namespace PrinterAgent.Test
{
    [TestClass]
    public class GhostScriptTest
    {
        [TestMethod]
        public void Print()
        {
            string printerName = "Bullzip PDF Printer";         
            string filePath = @"C:\Users\jevgenisa\Desktop\pdf.pdf";

            byte[] bytes = File.ReadAllBytes(filePath);

            new GhostScriptPrintingHandler().Print(printerName, bytes, Guid.NewGuid().ToString());
        }
    }
}
