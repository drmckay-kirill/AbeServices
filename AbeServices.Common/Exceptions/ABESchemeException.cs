using System;

namespace AbeServices.Common.Exceptions
{
    public class ABESchemeException: Exception
    {
        public ABESchemeException(string message, Exception exception) : base(message, exception) { }
    }
}