﻿namespace PrinterAgentServer.Exception
{
    public class UnathorizedException : System.Exception
    {
        public UnathorizedException(string message) : base(message)
        {
            
        }
    }

    public class ForbiddenException : System.Exception
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }
}
