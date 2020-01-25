using System;
using System.Threading.Tasks;
using MongoDB.Driver;
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
        private readonly IOptions<DatabaseSettings> _dbOptions;
        private readonly IMongoCollection<Session> _sessions;


        public TokensService(IAbeAuthBuilder abeAuthBuilder, 
            IAbeDecorator abeDecorator,
            IOptions<MainSettings> options,
            IOptions<DatabaseSettings> dbOptions)
        {
            _abeAuthBuilder = abeAuthBuilder;
            _abeDecorator = abeDecorator;
            _options = options;
            _dbOptions = dbOptions;

            var client = new MongoClient(_dbOptions.Value.ConnectionString);
            var database = client.GetDatabase(_dbOptions.Value.DatabaseName);
            _sessions = database.GetCollection<Session>("sessions");
        }

        public async Task<(byte[], string)> ProcessTokenRequest(byte[] requestData, string inputSessionId)
        {
            Guid sessionId = String.IsNullOrEmpty(inputSessionId) 
                ? Guid.Empty 
                : Guid.Parse(inputSessionId);
            var session = await _sessions
                .Find(x => x.Id == sessionId)
                .FirstOrDefaultAsync();

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
                await _sessions.InsertOneAsync(session);
                return (protocolStep, session.Id.ToString());
            }
            else
            {
                if (!session.IsProcessed)
                {
                    var request = _abeAuthBuilder.GetStepData<AbeAuthStepFour>(requestData);
                    
                    var nonceHash = CryptoHelper.ComputeHash($"{session.Nonce2}{session.Nonce3}");
                    if (!nonceHash.SequenceEqual(request.NonceHash))
                        throw new ProtocolArgumentException("Nonce hash in incorrect!");

                    var protocolStep = await _abeAuthBuilder.BuildStepFive(
                            session.AbonentAttributes,
                            _options.Value.IoTASharedKey,
                            session.HMAC);

                    session.IsProcessed = true;

                    var filter = Builders<Session>.Filter.Eq("Id", session.Id);
                    var update = Builders<Session>.Update.Set("IsProcessed", true);
                    await _sessions.UpdateOneAsync(filter, update);

                    return (protocolStep, session.Id.ToString());
                }
            }

            return (null, "");
        }
    }
}