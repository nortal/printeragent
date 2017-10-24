using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrinterAgent.PrintingHandler;

namespace PrinterAgent.Test
{
    [TestClass]
    public class GhostScriptTest
    {
        [TestMethod]
        public void Print()
        {
            //string printerName = "HP LaserJet Professional P1606dn";
            string printerName = "Bullzip PDF Printer";
            //string printerName = "\\\\tarp\\3 floor corridor printer";
            string filePath = @"C:\Users\jevgenisa\Desktop\test_095.pdf";
            //string filePath = @"C:\Users\jevgenisa\Desktop\pdf.pdf";
            byte[] bytes = File.ReadAllBytes(filePath);

            new GhostScriptPrintingHandler().Print(printerName, bytes);
        }
    }
}
