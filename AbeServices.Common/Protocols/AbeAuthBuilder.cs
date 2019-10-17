using System;
using System.Threading.Tasks;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.Common.Protocols
{
    public class AbeAuthBuilder : IAbeAuthBuilder
    {
        private readonly IDataSerializer _serializer;

        public AbeAuthBuilder(IDataSerializer serializer)
        {
            _serializer = serializer;
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
            int nonce = CryptoHelper.GetNonce();
            var payload = new AbeAuthStepTwo()
            {
                AccessPolicy = accessPolicy,
                AbonentAttributes = abonentAttr,
                CT = null,
                Z = Z
            };
            var res = _serializer.Serialize<AbeAuthStepTwo>(payload);
            return (res, nonce);
        }
    }
}