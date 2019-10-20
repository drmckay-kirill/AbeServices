using System.Threading.Tasks;

namespace AbeServices.Common.Protocols
{
    public interface IAbeAuthBuilder
    {
         T GetStepData<T>(byte[] data);
        byte[] BuildStepOne(string[] accessPolicy, string sharedKey);

        Task<(byte[], int)> BuildStepTwo(string[] accessPolicy, string[] abonentAttr, string[] tgsAttr, byte[] Z);
        Task<(byte[], int, int)> BuildStepThree(string[] accessPolicy, string[] abonentAttr, int nonce);
        Task<byte[]> BuildStepFour(int nonceAbonent,int nonceAccess);
    }
}