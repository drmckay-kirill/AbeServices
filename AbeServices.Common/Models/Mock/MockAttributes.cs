using System;
using AbeServices.Common.Models.Base;
using System.Collections.Generic;

namespace AbeServices.Common.Models.Mock
{
    public class MockAttributes : IAttributes
    {

        public MockAttributes()
        {
            _attributes = new List<string>();
        }

        public MockAttributes(string attributes)
        {
            _attributes = new List<string>();
            _attributes.AddRange(attributes.Split(' '));
        }

        public string Get()
        {
            return String.Join(" ", _attributes);
        }

        private List<string> _attributes; 
    }
}