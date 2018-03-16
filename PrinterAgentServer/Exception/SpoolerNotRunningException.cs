using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgentServer.Exception
{
    public class SpoolerNotRunningException : AgentFatalException
    {
        public override int GetCode()
        {
            return (int)AgentExceptionCode.SpoolerNotRunning;
        }
    }
}
