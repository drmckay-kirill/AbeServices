namespace AbeServices.Common.Protocols
{
    public interface IKeyDistributionBuilder
    {
        T GetStepData<T>(byte[] data);
        T GetPayload<T>(byte[] data, string key = null);

        (byte[], int) BuildStepOne(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes);

        (byte[], int) BuildStepTwo(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes, byte[] abonentPayload);

        byte[] BuildStepThree(string abonentKey, string serviceKey, int abonentNonce, int serviceNonce, byte[] PublicKey, byte[] SecretKey);
    }
}