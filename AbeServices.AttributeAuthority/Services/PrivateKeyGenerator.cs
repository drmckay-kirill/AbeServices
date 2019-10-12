using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Protocols;
using AbeServices.AttributeAuthority.Models;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Models.Base;
using AbeServices.Common.Exceptions;

namespace AbeServices.AttributeAuthority.Services
{
    public class PrivateKeyGenerator : IPrivateKeyGenerator
    {
        private readonly IOptions<MainSettings> _settings;
        private readonly IKeyDistributionBuilder _builder;
        private readonly ILoginService _loginService;
        private readonly MockCPAbe cpabeCenter;
        private IMasterKey masterKey;
        private IPublicKey publicKey;

        public PrivateKeyGenerator(IOptions<MainSettings> settings,
            IKeyDistributionBuilder builder,
            ILoginService loginService)
        {
            _settings = settings;
            _builder = builder;
            _loginService = loginService;

            cpabeCenter = new MockCPAbe();
        }

        public async Task<byte[]> Generate(byte[] data)
        {
            await CheckKeys();

            var authRequest = _builder.GetStepData<KeyDistributionStepTwo>(data);
            
            var abonent = await _loginService.GetLogin(authRequest.AbonentId);
            var keyService = await _loginService.GetLogin(authRequest.KeyServiceId);

            var abonentPayload = _builder.GetPayload<KeyDistributionRequestPayload>(authRequest.AbonentPayload, abonent.SharedKey);
            var servicePayload = _builder.GetPayload<KeyDistributionRequestPayload>(authRequest.KeyServicePayload, keyService.SharedKey);

            if (abonentPayload.AttributeAuthorityId != _settings.Value.Name)
                throw new ProtocolArgumentException("Incorrect attribute authority id");

            if (servicePayload.AttributeAuthorityId != _settings.Value.Name)
                throw new ProtocolArgumentException("Incorrect attribute authority id");

            if (abonentPayload.AbonentId != servicePayload.AbonentId)
                throw new ProtocolArgumentException("Incorrect abonent id");

            if (abonentPayload.KeyServiceId != servicePayload.KeyServiceId)
                throw new ProtocolArgumentException("Incorrect key service id");

            if (!abonentPayload.Attributes.SequenceEqual(servicePayload.Attributes))
                throw new ProtocolArgumentException("Incorrect attributes");

            if (abonentPayload.Attributes
                .Where(attr => !abonent.Attributes.Contains(attr))
                .Any())
            {
                throw new ProtocolArgumentException("Attribute access error");   
            }
            
            var secretKey = await cpabeCenter.Generate(masterKey, publicKey, new MockAttributes(abonentPayload.Attributes));

            var authResponse = _builder.BuildStepThree(abonent.SharedKey, 
                keyService.SharedKey, 
                abonentPayload.Nonce, 
                servicePayload.Nonce,
                publicKey.Value, 
                secretKey.Value);
            
            return authResponse;
        }

        private async Task CheckKeys()
        {
            if (publicKey == null)
            {
                var keys = await cpabeCenter.Setup();
                publicKey = keys.PublicKey;
                masterKey = keys.MasterKey;
            }
        }
    }
}