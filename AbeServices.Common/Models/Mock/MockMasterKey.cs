using AbeServices.Common.Models.Base;

namespace AbeServices.Common.Models.Mock
{
    public class MockMasterKey : IMasterKey 
    { 
        public byte[] Value { get; set;}
    }
}