using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using AbeServices.IoTA.Models;
using AbeServices.Common.Protocols;
using AbeServices.IoTA.Settings;

namespace AbeServices.IoTA.Services
{
    public class FiwareService : IFiwareService
    {
        private readonly IEntityService _entityService;
        private readonly IAbeAuthBuilder _abeAuthBuilder;
        private readonly IOptions<MainSettings> _options;
        private List<Session> sessions;

        public FiwareService(IEntityService entityService, 
            IAbeAuthBuilder abeAuthBuilder,
            IOptions<MainSettings> options)
        {
            _entityService = entityService;
            _abeAuthBuilder = abeAuthBuilder;
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
                return (_abeAuthBuilder.BuildStepOne(attributes, _options.Value.SGTSharedKey), session.Id);
            }
            
            return (null, session.Id);
        }
    }
}