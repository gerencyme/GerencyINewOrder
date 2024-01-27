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





        /*public async Task<List<DailyOrderTotal>> GetLast7DaysOrderTotal(string cnpj)
        {
            var endDate = DateTime.UtcNow;  // Data atual
            var startDate = endDate.AddDays(-7);  // Retrocede 7 dias

            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj),
                Builders<NewOrder>.Filter.Gte("OrderDate", startDate),
                Builders<NewOrder>.Filter.Lte("OrderDate", endDate)
            );

            var group = new BsonDocument
    {
        {
            "$group", new BsonDocument
            {
                { "_id", new BsonDocument("$dateToString", new BsonDocument("format", "%Y-%m-%d").Add("date", "$OrderDate")) },
                { "totalOrders", new BsonDocument("$sum", 1) }
            }
        }
    };

            var sort = Builders<BsonDocument>.Sort.Ascending("_id");

            var pipeline = PipelineDefinition<NewOrder, DailyOrderTotal>.Create(
                new IPipelineStageDefinition[]
                {
            PipelineStageDefinitionBuilder.Match(filter),
            PipelineStageDefinitionBuilder.Group(group),
            PipelineStageDefinitionBuilder.Sort(sort)
                }
            );

            var result = await _collection.Aggregate(pipeline).ToListAsync();

            var dailyOrderTotals = result.Select(doc => new DailyOrderTotal
            {
                Date = DateTime.Parse(doc["_id"].AsString),
                TotalOrders = doc["totalOrders"].AsInt32
            }).ToList();

            return dailyOrderTotals;
        }*/




        public async Task UpdateNewOrder(NewOrder entity, Guid orderId)
        {
            var filter = Builders<NewOrder>.Filter.Eq("OrderId", orderId);
            var existingEntity = await _collection.Find(filter).FirstOrDefaultAsync();

            if (existingEntity != null)
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


        public async Task<List<NewOrder>> GetLast12NewOrders(string cnpj)
        {
            var filter = Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj);
            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(filter)
                                   .Sort(sort)
                                   .Limit(12)
                                   .ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByDateRange(string cnpj, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj),
                Builders<NewOrder>.Filter.Gte("OrderDate", startDate),
                Builders<NewOrder>.Filter.Lte("OrderDate", endDate)
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(filter)
                                   .Sort(sort)
                                   .ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByDateRangeWithPagination(string cnpj, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj),
                Builders<NewOrder>.Filter.Gte("OrderDate", startDate),
                Builders<NewOrder>.Filter.Lte("OrderDate", endDate)
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            var query = _collection.Find(filter)
                                   .Sort(sort)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Limit(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByDateWithPagination(string cnpj, int pageNumber, int pageSize)
        {
            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj)
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            var query = _collection.Find(filter)
                                   .Sort(sort)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Limit(pageSize);

            return await query.ToListAsync();
        }


        public async Task<List<NewOrder>> GetSimilarOrdersUnderAnalysis(string cnpj, string orderId)
        {
            // Encontrar o pedido original
            var originalOrderFilter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq("CompanieCNPJ", cnpj),
                Builders<NewOrder>.Filter.Eq("OrderId", orderId)
            );

            var originalOrder = await _collection.Find(originalOrderFilter).FirstOrDefaultAsync();

            if (originalOrder == null)
            {
                // Pedido original não encontrado
                return new List<NewOrder>();
            }

            // Encontrar pedidos com os mesmos produtos e status "underAnalysis"
            var similarOrdersFilter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.ElemMatch("Product", Builders<Product>.Filter.Eq("ProductName", originalOrder.Product.ProductName)),
                Builders<NewOrder>.Filter.Eq("StatusOrder", "underAnalysis"),
                Builders<NewOrder>.Filter.Ne("OrderId", orderId)  // Excluir o pedido original
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(similarOrdersFilter)
                                   .Sort(sort)
                                   .ToListAsync();
        }
    }
}
