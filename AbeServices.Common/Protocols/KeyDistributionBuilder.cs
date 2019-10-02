using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.Common.Protocols
{
    public class KeyDistributionBuilder: IKeyDistributionBuilder
    {
        private IDataSerializer _serializer;
        private IDataSymmetricEncryptor _encryptor;

        public KeyDistributionBuilder(IDataSerializer serializer, IDataSymmetricEncryptor encryptor)
        {
            _serializer = serializer;
            _encryptor = encryptor;
        }

        public byte[] BuildStepOne(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes)
        {
            _encryptor.SetKey(key);

            var payload = new KeyDistributionStepOnePackage()
            {
                AbonentId = abonentId,
                KeyServiceId = keyServiceId,
                AttributeAuthorityId = authorityId,
                Attributes = abonentAttributes,
                Nonce = CryptoHelper.GetNonce()
            };
            var serializedPayload = _serializer.Serialize<KeyDistributionStepOnePackage>(payload);
            var encryptedPayload = _encryptor.Encrypt(serializedPayload);
            var stepData = new KeyDistrubtionStepOne()
            {
                AbonentId = abonentId,
                AttributeAuthorityId = authorityId,
                Attributes = abonentAttributes,
                Payload = encryptedPayload
            };
            var serializedData = _serializer.Serialize<KeyDistrubtionStepOne>(stepData);
            return serializedData;
        }
    }
}