using AbeServices.Common.Models.Base;

namespace AbeServices.Common.Models.Mock
{
    public class MockPublicKey : IPublicKey 
    { 
        public byte[] Value { get; set;}
    }
}