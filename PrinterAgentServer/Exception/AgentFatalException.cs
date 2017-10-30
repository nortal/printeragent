using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgentServer.Exception
{
    public abstract class AgentFatalException : System.Exception
    {
        public abstract int GetCode();
    }
}
