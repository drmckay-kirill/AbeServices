using System;

namespace AbeServices.Common.Exceptions
{
    public class ProtocolArgumentException : Exception
    {
        public ProtocolArgumentException(string message) : base(message) { }
    }
}