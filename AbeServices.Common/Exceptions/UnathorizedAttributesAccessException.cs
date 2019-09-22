using System;

namespace AbeServices.Common.Exceptions
{
    public class UnathorizedAttributesAccessException: Exception
    {
        public UnathorizedAttributesAccessException() : base("Unauthorized access to message: cannot decrypt") { }
    }
}