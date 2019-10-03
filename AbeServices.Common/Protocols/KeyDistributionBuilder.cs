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

        public KeyDistrubutionStepOne GetStepOne(byte[] data)
        {
            return _serializer.Deserialize<KeyDistrubutionStepOne>(data);
        }

        public KeyDistributionStepTwo GetStepTwo(byte[] data)
        {
            return _serializer.Deserialize<KeyDistributionStepTwo>(data);
        }

        public KeyDistributionStepThree GetStepThree(byte[] data)
        {
            return _serializer.Deserialize<KeyDistributionStepThree>(data);
        }

        public KeyDistributionRequestPayload GetRequestPayload(byte[] data, string key = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _encryptor.SetKey(key);
                var decryptedData = _encryptor.Decrypt(data);
                return _serializer.Deserialize<KeyDistributionRequestPayload>(decryptedData);
            }
            else
            {
                return _serializer.Deserialize<KeyDistributionRequestPayload>(data);
            }
        }

        // Вызывается абонентом, запрашивающим ключ
        public byte[] BuildStepOne(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes)
        {
            _encryptor.SetKey(key);

            var payload = new KeyDistributionRequestPayload()
            {
                AbonentId = abonentId,
                KeyServiceId = keyServiceId,
                AttributeAuthorityId = authorityId,
                Attributes = abonentAttributes,
                Nonce = CryptoHelper.GetNonce()
            };
            var serializedPayload = _serializer.Serialize<KeyDistributionRequestPayload>(payload);
            var encryptedPayload = _encryptor.Encrypt(serializedPayload);
            var stepData = new KeyDistrubutionStepOne()
            {
                AbonentId = abonentId,
                AttributeAuthorityId = authorityId,
                Attributes = abonentAttributes,
                Payload = encryptedPayload
            };
            var serializedData = _serializer.Serialize<KeyDistrubutionStepOne>(stepData);
            return serializedData;
        }

        // Вызывается промежуточным сервисом раздачи ключей
        public byte[] BuildStepTwo(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes, byte[] abonentPayload)
        {
            _encryptor.SetKey(key);

            var servicePayload = new KeyDistributionRequestPayload()
            {
                AbonentId = abonentId,
                KeyServiceId = keyServiceId,
                AttributeAuthorityId = authorityId,
                Attributes = abonentAttributes,
                Nonce = CryptoHelper.GetNonce()
            };
            var serializedServicePayload = _serializer.Serialize<KeyDistributionRequestPayload>(servicePayload);
            var encryptedServicePayload = _encryptor.Encrypt(serializedServicePayload);
            var stepData = new KeyDistributionStepTwo()
            {
                KeyServiceId = keyServiceId,
                AbonentId = abonentId,
                AbonentPayload = abonentPayload,
                KeyServicePayload = encryptedServicePayload
            };
            var serializedData = _serializer.Serialize<KeyDistributionStepTwo>(stepData);
            return serializedData;
        }

        // Вызывается атрибутивным центром - центром генерации ключей
        public byte[] BuildStepThree(string abonentKey, string serviceKey, int abonentNonce, int serviceNonce, byte[] publicKey, byte[] secretKey)
        {
            _encryptor.SetKey(abonentKey);
            var abonentPayload = new KeyDistributionAuthToAbonent()
            {
                Nonce = abonentNonce,
                PublicKey = publicKey,
                SecretKey = secretKey
            };
            var serializedAbonentPayload = _serializer.Serialize<KeyDistributionAuthToAbonent>(abonentPayload);
            var encryptedAbonentPayload = _encryptor.Encrypt(serializedAbonentPayload);

            _encryptor.SetKey(serviceKey);
            var servicePayload = new KeyDistributionAuthToService()
            {
                Nonce = serviceNonce
            };
            var serializedServicePayload = _serializer.Serialize<KeyDistributionAuthToService>(servicePayload);
            var encryptedServicePayload = _encryptor.Encrypt(serializedServicePayload);

            var stepData = new KeyDistributionStepThree()
            {
                AbonentPayload = encryptedAbonentPayload,
                ServicePayload = encryptedServicePayload
            };
            var serializedData = _serializer.Serialize<KeyDistributionStepThree>(stepData);
            return serializedData;
        }
    }
}