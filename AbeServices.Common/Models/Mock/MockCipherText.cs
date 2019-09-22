using AbeServices.Common.Models.Base;

namespace AbeServices.Common.Models.Mock
{
    public class MockCipherText : ICipherText
    { 
        public byte[] Value { get; set;}
    }
}