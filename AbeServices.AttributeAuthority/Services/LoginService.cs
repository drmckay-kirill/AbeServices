using AbeServices.AttributeAuthority.Models;
using System.Collections.Generic;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AbeServices.AttributeAuthority.Services
{
    public class LoginService : ILoginService
    {
        private readonly IMongoCollection<Login> _logins;

        public LoginService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _logins = database.GetCollection<Login>(settings.Value.LoginCollectionName);
        }

        public async Task<Login> GetLogin(string name)
        {
            return await _logins
                .Find(login => login.Name == name)
                .FirstAsync();
        }

        public async Task<List<Login>> GetList()
        {
            return await _logins
                    .Find(login => true)
                    .SortBy(login => login.Name)
                    .ToListAsync();
        }

        public async Task Create(Login login)
        {
            await _logins.InsertOneAsync(login);
        }
    }
}