using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using AbeServices.IoTA.Models;
using AbeServices.IoTA.Settings;
using AbeServices.Common.Exceptions;

namespace AbeServices.IoTA.Services
{
    public class EntityService: IEntityService
    {
        private readonly IMongoCollection<Entity> _entities;

        public EntityService(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _entities = database.GetCollection<Entity>(settings.Value.DatabaseName);
        }

        public async Task Create(EntityViewModel entityViewModel)
        {
            var existingEntity = await _entities
                .Find(x => x.Name == entityViewModel.Name)
                .AnyAsync();
            
            if (existingEntity)
                throw new DuplicatedObjectException($"Entity with name {entityViewModel.Name} is already exists");

            var entity = new Entity()
            {
                Name = entityViewModel.Name,
                ReadAttributes = entityViewModel.ReadAttributes,
                WriteAttributes = entityViewModel.WriteAttributes
            };

            await _entities.InsertOneAsync(entity);
        }

        public async Task<EntityViewModel> Get(string name)
        {
            try
            {
                var entity = await _entities
                    .Find(x => x.Name == name)
                    .FirstAsync();
                var res = new EntityViewModel()
                {
                    Name = entity.Name,
                    ReadAttributes = entity.ReadAttributes,
                    WriteAttributes = entity.WriteAttributes
                };
                return res;
            }
            catch(InvalidOperationException)
            {
                throw new NotFoundException($"Entity with name {name} was not found");
            }
        }
    }
}