using System;
namespace PrinterAgent.Util
{
    public class UnathorizedException : Exception
    {
        public UnathorizedException(string message) : base(message)
        {
            
        }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }
}
