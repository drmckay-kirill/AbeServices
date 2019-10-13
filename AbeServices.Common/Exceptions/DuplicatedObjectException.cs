using System;

namespace AbeServices.Common.Exceptions
{
    public class DuplicatedObjectException : Exception
    {
        public DuplicatedObjectException(string message) : base(message) { }
    }
}