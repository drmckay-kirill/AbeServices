using AbeServices.Common.Models.Protocols;

namespace AbeServices.Common.Protocols
{
    public interface IKeyDistributionBuilder
    {
        KeyDistrubutionStepOne GetStepOne(byte[] data);
        KeyDistributionStepTwo GetStepTwo(byte[] data);
        KeyDistributionStepThree GetStepThree(byte[] data);
        KeyDistributionRequestPayload GetRequestPayload(byte[] data, string key = null);

        // ���������� ���������, ������������� ����
         byte[] BuildStepOne(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes);

        // ���������� ������������� �������� ������� ������
        byte[] BuildStepTwo(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes, byte[] abonentPayload);

        // ���������� ������������ ������� - ������� ��������� ������
        byte[] BuildStepThree(string abonentKey, string serviceKey, int abonentNonce, int serviceNonce, byte[] PublicKey, byte[] SecretKey);
    }
}