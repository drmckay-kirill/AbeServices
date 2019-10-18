using System;
using System.Threading.Tasks;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.Common.Protocols
{
    public class AbeAuthBuilder : IAbeAuthBuilder
    {
        private readonly IDataSerializer _serializer;
        private readonly IAbeDecorator _abeDecorator;

        public AbeAuthBuilder(IDataSerializer serializer, IAbeDecorator abeDecorator)
        {
            _serializer = serializer;
            _abeDecorator = abeDecorator;
        }

        public T GetStepData<T>(byte[] data)
        {
            return _serializer.Deserialize<T>(data);
        }

        public byte[] BuildStepOne(string[] accessPolicy, string sharedKey)
        {
            var hash = CryptoHelper.ComputeHash($"{string.Join("", accessPolicy)}{sharedKey}");
            var payload = new AbeAuthStepOne()
            {
                AccessPolicy = accessPolicy,
                Z = hash
            };
            return _serializer.Serialize<AbeAuthStepOne>(payload);
        }

        public async Task<(byte[], int)> BuildStepTwo(string[] accessPolicy, string[] abonentAttr, string[] tgsAttr, byte[] Z)
        {
            await _abeDecorator.Setup();

            int nonce = CryptoHelper.GetNonce();

            var cipherTextBytes = await _abeDecorator.Encrypt(BitConverter.GetBytes(nonce), tgsAttr);

            var payload = new AbeAuthStepTwo()
            {
                AccessPolicy = accessPolicy,
                AbonentAttributes = abonentAttr,
                CT = cipherTextBytes,
                Z = Z
            };
            var res = _serializer.Serialize<AbeAuthStepTwo>(payload);
            return (res, nonce);
        }
    }
}