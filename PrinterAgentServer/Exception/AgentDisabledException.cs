using System;
using System.Runtime.Serialization;

namespace PrinterAgentServer.Exception
{
    [Serializable]
    public class AgentDisabledException : AgentFatalException
    {
        public override int GetCode()
        {
            return (int) AgentExceptionCode.AgentDisabled;
        }
    }
}