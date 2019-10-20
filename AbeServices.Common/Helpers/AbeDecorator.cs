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
        public static class Factory
        {
            public static AbeDecorator Create(string SharedKey, string Id, 
                string KeyServiceId, string AuthorityId, string[] Attributes, string KeyServiceUrl)
            {
                var res =  new AbeDecorator();
                res.SharedKey = SharedKey;
                res.Id = Id;
                res.KeyServiceId = KeyServiceId;
                res.AuthorityId = AuthorityId;
                res.Attributes = Attributes;
                res.KeyServiceUrl = KeyServiceUrl;
                return res;
            }
        }

        public string SharedKey { get; set; }
        public string Id { get; set; }
        public string KeyServiceId { get; set; }
        public string AuthorityId { get; set; }
        public string[] Attributes { get; set; }
        public string KeyServiceUrl { get; set;}
        private readonly IOptions<AbeSettings> _options;
        private readonly IKeyDistributionBuilder _builder;
        private readonly MockCPAbe cpabeCenter;
        private IPublicKey publicKey;
        private ISecretKey secretKey;

        public AbeDecorator() 
        { 
            _builder = new KeyDistributionBuilder(new ProtobufDataSerializer(), new DataSymmetricEncryption());
            cpabeCenter = new MockCPAbe();
        }

        public AbeDecorator(IOptions<AbeSettings> options,
            IKeyDistributionBuilder builder)
        {
            _options = options;
            _builder = builder;
            cpabeCenter = new MockCPAbe();

            SharedKey = _options.Value.SharedKey;
            Id = _options.Value.Id;
            KeyServiceId = _options.Value.KeyServiceId;
            AuthorityId = _options.Value.AuthorityId;
            Attributes = _options.Value.Attributes;
            KeyServiceUrl = _options.Value.KeyServiceUrl;
        }

        public async Task<byte[]> Encrypt(byte[] data, string[] accessStructure)
        {   
            await Setup();

            var accessPolicy = new MockAttributes(accessStructure);
            var cipherText = await cpabeCenter.Encrypt(data, publicKey, accessPolicy);
            return cipherText.Value;
        }

        public async Task<byte[]> Decrypt(byte[] data)
        {
            await Setup();

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
                    _builder.BuildStepOne(SharedKey, 
                        Id, 
                        KeyServiceId, 
                        AuthorityId, 
                        Attributes);

                var requestContent = new ByteArrayContent(requestData);
                var client = new HttpClient();
                var response = await client.PostAsync(KeyServiceUrl, requestContent);

                var responseData = await response.Content.ReadAsByteArrayAsync(); 
                var payload = _builder.GetPayload<KeyDistributionAuthToAbonent>(responseData, SharedKey);

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