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

        public async Task<(byte[], int, int)> BuildStepThree(string[] accessPolicy, string[] abonentAttr, int nonce)
        {
            await _abeDecorator.Setup();

            var nonceAbonent = CryptoHelper.GetNonce();
            var nonceAccess = CryptoHelper.GetNonce();

            var payload = new AbeAuthStepThree()
            {
                Nonce = nonce,
                CtAbonent = await _abeDecorator.Encrypt(BitConverter.GetBytes(nonceAbonent), abonentAttr),
                CtAccess = await _abeDecorator.Encrypt(BitConverter.GetBytes(nonceAccess), accessPolicy),
                NonceHash = CryptoHelper.ComputeHash($"{nonce}{nonceAbonent}{nonceAccess}")
            };
            var res = _serializer.Serialize<AbeAuthStepThree>(payload);

            return (res, nonceAbonent, nonceAccess);
        }

        public async Task<byte[]> BuildStepFour(int nonceAbonent,int nonceAccess)
        {
            var payload = new AbeAuthStepFour()
            {
                NonceHash = CryptoHelper.ComputeHash($"{nonceAbonent}{nonceAccess}")
            };
            var res = _serializer.Serialize<AbeAuthStepFour>(payload);
            return res;
        }
    }
}