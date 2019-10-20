using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Exceptions;
using AbeServices.TokenGeneration.Models;
using AbeServices.TokenGeneration.Settings;

namespace AbeServices.TokenGeneration.Services
{
    public class TokensService : ITokensService
    {
        private readonly IAbeAuthBuilder _abeAuthBuilder;
        private readonly IAbeDecorator _abeDecorator;
        private readonly IOptions<MainSettings> _options;
        private List<Session> sessions;

        public TokensService(IAbeAuthBuilder abeAuthBuilder, 
            IAbeDecorator abeDecorator,
            IOptions<MainSettings> options)
        {
            _abeAuthBuilder = abeAuthBuilder;
            _abeDecorator = abeDecorator;
            _options = options;
            sessions = new List<Session>();
        }

        public async Task<(byte[], string)> ProcessTokenRequest(byte[] requestData, string inputSessionId)
        {
            Guid sessionId = String.IsNullOrEmpty(inputSessionId) 
                ? Guid.Empty 
                : Guid.Parse(inputSessionId);
            var session = sessions.Find(x => x.Id == sessionId);

            if (session == null)
            {
                var request = _abeAuthBuilder.GetStepData<AbeAuthStepTwo>(requestData);
                
                var Z = CryptoHelper.ComputeHash($"{string.Join("", request.AccessPolicy)}{_options.Value.IoTASharedKey}");           
                if (!Z.SequenceEqual(request.Z))
                    throw new ProtocolArgumentException("HMAC from policy enforcement point is incorrect!");

                var decryptedNonce = await _abeDecorator.Decrypt(request.CT);
                var requestNonce = BitConverter.ToInt32(decryptedNonce, 0);
                var (protocolStep, nonceAbonent, nonceAccess) = 
                    await _abeAuthBuilder.BuildStepThree(request.AccessPolicy,
                            request.AbonentAttributes, requestNonce);
                
                session = new Session()
                {
                    Id = Guid.NewGuid(),
                    AccessPolicy = request.AccessPolicy,
                    AbonentAttributes = request.AbonentAttributes,
                    Nonce1 = requestNonce,
                    Nonce2 = nonceAbonent,
                    Nonce3 = nonceAccess,
                    HMAC = Z
                };
                sessions.Add(session);
                return (protocolStep, session.Id.ToString());
            }

            return (null, "");
        }
    }
}