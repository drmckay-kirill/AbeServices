using AbeServices.Common.Models.Base;

namespace AbeServices.Common.Models.Mock
{
    public class MockSecretKey : ISecretKey
    {
        public byte[] Value { get; set;}
    }
}