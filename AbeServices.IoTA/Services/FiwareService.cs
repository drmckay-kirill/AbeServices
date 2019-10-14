namespace AbeServices.IoTA.Services
{
    public class FiwareService : IFiwareService
    {
        private readonly IEntityService _entityService;

        public FiwareService(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public byte[] Authorize(string entityName, string sessionId, byte[] body, bool read)
        {
            // List<AuthSession> - Id, Step, Key
            // запрос к таблице сессий, на основании результата выполнять шаги

            var entity = _entityService.Get(entityName).Result;
            var attributes = read ? entity.ReadAttributes : entity.WriteAttributes;
            
            return System.Text.Encoding.UTF8.GetBytes(string.Join(" ", attributes));
        }
    }
}