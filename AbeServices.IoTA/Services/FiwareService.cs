using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using AbeServices.IoTA.Models;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;
using AbeServices.Common.Exceptions;
using AbeServices.Common.Models.Protocols;
using AbeServices.IoTA.Settings;

namespace AbeServices.IoTA.Services
{
    public class FiwareService : IFiwareService
    {
        private readonly IEntityService _entityService;
        private readonly IAbeAuthBuilder _abeAuthBuilder;
        private readonly IDataSymmetricEncryptor _encryptor;
        private readonly IOptions<MainSettings> _options;
        private List<Session> sessions;

        public FiwareService(IEntityService entityService, 
            IAbeAuthBuilder abeAuthBuilder,
            IDataSymmetricEncryptor encryptor,
            IOptions<MainSettings> options)
        {
            _entityService = entityService;
            _abeAuthBuilder = abeAuthBuilder;
            _encryptor = encryptor;
            _options = options;
            sessions = new List<Session>();
        }

        public async Task<(byte[], Guid)> Authorize(string entityName, string sessionIdValue, byte[] body, bool read)
        {
            var sessionId = String.IsNullOrEmpty(sessionIdValue) 
                ? Guid.Empty 
                : Guid.Parse(sessionIdValue);
            var session = sessions.Find(x => x.Id == sessionId);
            
            if (session == null)
            {
                var entity = await _entityService.Get(entityName);
                session = new Session();
                sessions.Add(session);
                var attributes = read ? entity.ReadAttributes : entity.WriteAttributes;
                var (protocolStep, Z) = _abeAuthBuilder.BuildStepOne(attributes, _options.Value.SGTSharedKey);
                session.Z = Z;
                return (protocolStep, session.Id);
            }
            else
            {
                if (session.ProtocolStep == AbeAuthSteps.GetAccessPolicy)
                {
                    var request = _abeAuthBuilder.GetStepData<AbeAuthStepSix>(body);
                    _encryptor.SetKey(_options.Value.SGTSharedKey);
                    var sharedKey = _encryptor.Decrypt(request.CtPep);

                    var hmac = CryptoHelper.ComputeHash(session.Z, sharedKey);
                    if (!hmac.SequenceEqual(request.HMAC))
                        throw new ProtocolArgumentException("HMAC is incorrect!");

                    session.SharedKey = sharedKey;
                    session.ProtocolStep = AbeAuthSteps.ConfirmAccessPolicy;

                    var protocolStep = _abeAuthBuilder.BuildStepSeven(request.HMAC, sharedKey);
                    return (protocolStep, session.Id);
                }
                else
                {
                    // check hmac from header
                    // throw if need
                }
            }
            
            return (null, session.Id);
        }
    }
}