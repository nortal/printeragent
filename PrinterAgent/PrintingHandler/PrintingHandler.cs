using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgent.PrintingHandler
{
    interface PrintingHandler
    {
        void Print(string printerName, byte[] document);
    }
}
