using Domain.Interfaces.IRepositorys;
using Entities;
using Entities.Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repository.Repositories
{
    public class NewOrderRepository : RepositoryMongoDBGeneric<NewOrder>, IRepositoryNewOrder
    {
        private readonly IMongoCollection<NewOrder> _collection;

        public NewOrderRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<NewOrder>(typeof(NewOrder).Name);
        }

        public async Task<List<Product>> GetTop10Products()
        {
            var pipeline = new BsonDocument[]
        {
        new BsonDocument
        {
            { "$unwind", "$Product" }
        },
        new BsonDocument
        {
            { "$group", new BsonDocument
                {
                    { "totalPedidos", new BsonDocument("$sum", "$Product.Quantity") },
                    { "ProductName", new BsonDocument("$first", "$Product.ProductName") },
                    { "ProductBrand", new BsonDocument("$first", "$Product.ProductBrand") },
                    { "ProductType", new BsonDocument("$first", "$Product.ProductType") },
                    { "Quantity", new BsonDocument("$first", "$Product.Quantity") },
                    { "LastTotalPrice", new BsonDocument("$first", "$Product.LastTotalPrice") }
                }
            }
        },
        new BsonDocument
        {
            { "$sort", new BsonDocument("totalPedidos", -1) }
        },
        new BsonDocument
        {
            { "$limit", 10 }
        }
        };

            return await _collection.Aggregate<Product>(pipeline).ToListAsync();
        }

        public async Task DeleteNewOrder(Guid id)
        {
            var filter = Builders<NewOrder>.Filter.Eq("OrderId", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<NewOrder> GetByIdNewOrder(Guid id)
        {
            var filter = Builders<NewOrder>.Filter.Eq("OrderId", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateNewOrder(NewOrder entity, Guid orderId)
        {
            var filter = Builders<NewOrder>.Filter.Eq("OrderId", orderId);
            var existingEntity = await _collection.Find(filter).FirstOrDefaultAsync();

            if (existingEntity != null && existingEntity.Id != null)
            {
                entity.Id = existingEntity.Id;
                entity.OrderId = existingEntity.OrderId;
                await _collection.ReplaceOneAsync(filter, entity);
            }
            else
            {
                // Lógica para lidar com o caso em que a entidade não foi encontrada
                // Você pode lançar uma exceção, fazer algo diferente, etc.
                // Neste exemplo, estou apenas registrando uma mensagem.
                Console.WriteLine($"Entidade com OrderId {orderId} não encontrada.");
            }
        }

        public async Task<ReplaceOneResult> Update(NewOrder entity, Guid orderId)
        {
            var filter = Builders<NewOrder>.Filter.Eq("OrderId", orderId);
            return await _collection.ReplaceOneAsync(filter, entity);
        }
    }
}
