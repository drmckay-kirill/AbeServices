using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Http;
using AbeServices.Common.Models;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Models.Base;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Protocols;

namespace AbeServices.Common.Helpers
{
    public class AbeDecorator : IAbeDecorator
    {
        private readonly IOptions<AbeSettings> _options;
        private readonly IKeyDistributionBuilder _builder;
        private readonly MockCPAbe cpabeCenter;
        private IPublicKey publicKey;
        private ISecretKey secretKey;

        public AbeDecorator(IOptions<AbeSettings> options,
            IKeyDistributionBuilder builder)
        {
            _options = options;
            _builder = builder;
            cpabeCenter = new MockCPAbe();
        }

        public async Task<byte[]> Encrypt(byte[] data, string[] accessStructure)
        {
            var accessPolicy = new MockAttributes(accessStructure);
            var cipherText = await cpabeCenter.Encrypt(data, publicKey, accessPolicy);
            return cipherText.Value;
        }

        public async Task<byte[]> Decrypt(byte[] data)
        {
            var cipherText = new MockCipherText();
            cipherText.Value = new byte[data.Length];
            data.CopyTo(cipherText.Value, 0);
            return await cpabeCenter.DecryptToBytes(cipherText, publicKey, secretKey);
        }

        public async Task Setup()
        {
            if (secretKey == null)
            {
                var (requestData, abonentNonce) = 
                    _builder.BuildStepOne(_options.Value.SharedKey, 
                        _options.Value.Id, 
                        _options.Value.KeyServiceId, 
                        _options.Value.AuthorityId, 
                        _options.Value.Attributes);

                var requestContent = new ByteArrayContent(requestData);
                var client = new HttpClient();
                var response = await client.PostAsync(_options.Value.KeyServiceUrl, requestContent);

                var responseData = await response.Content.ReadAsByteArrayAsync(); 
                var payload = _builder.GetPayload<KeyDistributionAuthToAbonent>(responseData, _options.Value.SharedKey);

                secretKey = new MockSecretKey();
                secretKey.Value = new byte[payload.SecretKey.Length];
                payload.SecretKey.CopyTo(secretKey.Value , 0);

                publicKey = new MockPublicKey();
                publicKey.Value = new byte[payload.PublicKey.Length];
                payload.PublicKey.CopyTo(publicKey.Value, 0);
            }
        }
    }
}