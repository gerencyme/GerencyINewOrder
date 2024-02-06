using Domain.Interfaces.IRepositorys;
using Entities;
using Entities.Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Infrastructure.Repository.Repositories
{
    public class NewOrderRepository : RepositoryMongoDBGeneric<NewOrder>, IRepositoryNewOrder
    {
        private readonly IMongoCollection<NewOrder> _collection;
        private readonly IClientSessionHandle _session;

        public NewOrderRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<NewOrder>(typeof(NewOrder).Name);
            _session = client.StartSession();
        }

        private const string CNPJ = "CompanieCNPJ";
        private const string ORDERID = "OrderId";

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

        public async Task<string> UpdateIsLikedField(string cnpj, Guid orderId, bool isLiked)
        {
            {
                try
                {
                    _session.StartTransaction();

                    var filter = Builders<NewOrder>.Filter.And(
                        Builders<NewOrder>.Filter.Eq(CNPJ, cnpj),
                        Builders<NewOrder>.Filter.Eq("OrderId", orderId)
                    );

                    var order = await _collection.Find(filter).FirstOrDefaultAsync();
                    if (order == null)
                    {
                        throw new Exception($"Entidade com OrderId {orderId} não encontrada.");
                    }

                    var product = order.Product;

                    var update = Builders<NewOrder>.Update
                        .Set("IsLiked", isLiked);

                    var result = await _collection.UpdateManyAsync(
                        Builders<NewOrder>.Filter.And(
                            Builders<NewOrder>.Filter.Eq(CNPJ, cnpj),
                            Builders<NewOrder>.Filter.Eq("Product.ProductName", product.ProductName)
                        ),
                        update
                    );

                    if (result.MatchedCount == 0)
                    {
                        throw new Exception($"Nenhum documento encontrado para a atualização.");
                    }

                    await _session.CommitTransactionAsync();

                    return $"Update {result.MatchedCount} IsLiked!";
                }
                catch (Exception)
                {
                    await _session.AbortTransactionAsync();
                    throw new Exception("Erro ao atualizar IsLiked. Todas as atualizações foram revertidas.");
                }
            }
        }

        public async Task<ReplaceOneResult> Update(NewOrder entity, Guid orderId)
        {
            var filter = Builders<NewOrder>.Filter.Eq(ORDERID, orderId);
            return await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteNewOrder(Guid id)
        {
            var filter = Builders<NewOrder>.Filter.Eq(ORDERID, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<NewOrder> GetByIdNewOrder(Guid id)
        {
            var filter = Builders<NewOrder>.Filter.Eq(ORDERID, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }


        public async Task<List<NewOrder>> GetLast12NewOrders(string cnpj)
        {
            var filter = Builders<NewOrder>.Filter.Eq(CNPJ, cnpj);
            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(filter)
                                   .Sort(sort)
                                   .Limit(12)
                                   .ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByDateRange(string cnpj, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq(CNPJ, cnpj),
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
                Builders<NewOrder>.Filter.Eq(CNPJ, cnpj),
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
                Builders<NewOrder>.Filter.Eq(CNPJ, cnpj)
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            var query = _collection.Find(filter)
                                   .Sort(sort)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Limit(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByIsLiked(string cnpj, bool isLiked)
        {
            var filter = Builders<NewOrder>.Filter.And(
                Builders<NewOrder>.Filter.Eq(CNPJ, cnpj),
                Builders<NewOrder>.Filter.Eq("IsLiked", isLiked)
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            var query = _collection.Find(filter)
                                   .Sort(sort);

            return await query.ToListAsync();
        }

        public async Task<List<NewOrder>> GetOrdersByProximity(double latitudeA, double longitudeA, double maxDistanceInMeters)
        {
            var filter = Builders<NewOrder>.Filter.NearSphere(
                x => x.Location,
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(longitudeA, latitudeA)
                ),
                maxDistanceInMeters
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<NewOrder>> GetSimilarOrdersUnderAnalysisFromList(List<NewOrder> orders)
        {
            // Encontrar o primeiro pedido com status "underAnalysis"
            var originalOrder = orders.FirstOrDefault(order => order.OrderStatus == "underAnalysis");

            if (originalOrder == null)
            {
                // Nenhum pedido em análise encontrado
                return new List<NewOrder>();
            }

            // Encontrar pedidos com os mesmos produtos e status "underAnalysis"
            return await _collection.Find(Builders<NewOrder>.Filter.And(
                    Builders<NewOrder>.Filter.ElemMatch("Product", Builders<Product>.Filter.Eq("ProductName", originalOrder.Product.ProductName)),
                    Builders<NewOrder>.Filter.Eq("StatusOrder", "underAnalysis")
                ))
                    .Sort(Builders<NewOrder>.Sort.Descending("OrderDate"))
                    .ToListAsync();


            /*var similarOrdersFilter = Builders<NewOrder>.Filter.And(
              Builders<NewOrder>.Filter.ElemMatch("Product", Builders<Product>.Filter.Eq("ProductName", originalOrder.Product.ProductName)),
              Builders<NewOrder>.Filter.Eq("StatusOrder", "underAnalysis")
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(similarOrdersFilter)
                        .Sort(sort)
                        .ToListAsync();*/
        }


        public async Task<List<NewOrder>> GroupAndAnalyzeOrdersByProximity(double maxDistanceInMeters)
        {
            maxDistanceInMeters = 25000;
            var listNewOrder = new List<NewOrder>();
            var orders = await GetSimilarOrdersUnderAnalysisFromList(listNewOrder);

            var groupedOrders = new List<NewOrder>();

            foreach (var order in orders)
            {
                // Filtrar pedidos semelhantes por proximidade
                var nearbyOrders = await GetOrdersByProximity(order.Location.Latitude, order.Location.Longitude, maxDistanceInMeters);

                // Verificar se há pedidos próximos
                if (nearbyOrders.Count > 0)
                {
                    // Encontrar o grupo prevalente usando o pedido de referência
                    var prevalentGroup = nearbyOrders.GroupBy(o => o.OrderId)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();

                    // Atualizar status dos pedidos no grupo prevalente
                    foreach (var groupedOrder in prevalentGroup)
                    {
                        groupedOrder.OrderStatus = "aguardando confirmação do fornecedor";
                        // Adicione aqui a lógica de persistência para salvar a atualização no banco de dados
                        await UpdateNewOrder(groupedOrder, groupedOrder.OrderId);
                    }

                    // Adicionar todos os pedidos próximos ao grupo geral
                    groupedOrders.AddRange(nearbyOrders);
                }
            }

            return groupedOrders;
        }


        public async Task<List<NewOrder>> GroupAndAnalyzeOrdersByProximity2()
        {
            const double maxDistanceInMeters = 25000;

            var pipeline = new List<BsonDocument>();

            // Etapa 1: Filtrar pedidos semelhantes por proximidade
            pipeline.Add(new BsonDocument
    {
        { "$geoNear", new BsonDocument
            {
                { "near", new BsonDocument
                    {
                        { "type", "Point" },
                        { "coordinates", new BsonArray { 0.0, 0.0 } } // Use suas coordenadas aqui
                    }
                },
                { "distanceField", "distance" },
                { "maxDistance", maxDistanceInMeters },
                { "spherical", true }
            }
        }
    });

            // Etapa 2: Filtrar pedidos semelhantes por status "underAnalysis"
            pipeline.Add(new BsonDocument
    {
        { "$match", new BsonDocument
            {
                { "StatusOrder", "underAnalysis" }
            }
        }
    });

            // Etapa 3: Agrupar pedidos por OrderId e contar a quantidade
            pipeline.Add(new BsonDocument
    {
        { "$group", new BsonDocument
            {
                { "_id", "$OrderId" },
                { "count", new BsonDocument { { "$sum", 1 } } }
            }
        }
    });

            // Etapa 4: Ordenar por contagem em ordem descendente
            pipeline.Add(new BsonDocument
    {
        { "$sort", new BsonDocument
            {
                { "count", -1 }
            }
        }
    });

            // Etapa 5: Projetar resultado final
            pipeline.Add(new BsonDocument
    {
        { "$project", new BsonDocument
            {
                { "_id", 0 },
                { "OrderId", "$_id" },
                { "count", 1 }
            }
        }
    });

            var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var groupedOrders = new List<NewOrder>();

            foreach (var doc in result)
            {
                var orderId = doc[ORDERID].AsInt32; // Substitua pelo tipo correto do seu ID
                var count = doc["count"].AsInt32;

                // Atualizar status para "aguardando confirmação do fornecedor"
                await UpdateOrderStatus(orderId, "Aguardando confirmação do fornecedor");

                // Adicionar à lista de pedidos agrupados
                groupedOrders.AddRange(await GetOrderDetails(orderId));
            }

            return groupedOrders;
        }

        private async Task UpdateOrderStatus(int orderId, string newStatus)
        {
            var filter = Builders<NewOrder>.Filter.Eq(ORDERID, orderId);
            var update = Builders<NewOrder>.Update.Set("StatusOrder", newStatus);
            await _collection.UpdateOneAsync(filter, update);
        }

        private async Task<List<NewOrder>> GetOrderDetails(int orderId)
        {
            var filter = Builders<NewOrder>.Filter.Eq(ORDERID, orderId);
            return await _collection.Find(filter).ToListAsync();
        }

    }
}


/*public async Task<List<NewOrder>> GroupAndAnalyzeOrdersByProximity(double maxDistanceInMeters)
        {
            
            maxDistanceInMeters = 25000;
            var listNewOrder = new List<NewOrder>();
            var orders = await GetSimilarOrdersUnderAnalysisFromList(listNewOrder);

            var groupedOrders = new List<NewOrder>();

            foreach (var order in orders)
            {
                // Filtrar pedidos semelhantes por proximidade
                var nearbyOrders = await GetOrdersByProximity(order.Location.Latitude, order.Location.Longitude, maxDistanceInMeters);

                // Adicionar lógica de comparação e agrupamento de pedidos aqui
                // Neste exemplo, estou apenas adicionando todos os pedidos semelhantes e próximos em um único grupo
                //groupedOrders.AddRange(similarOrders);
                groupedOrders.AddRange(nearbyOrders);
            }

            return groupedOrders;
        }*/


/*         public async Task<List<Product>> GetTop10Products()
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
 */

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



/*public async Task<List<NewOrder>> GetSimilarOrdersUnderAnalysis(string cnpj, string orderId)
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
                Builders<NewOrder>.Filter.Eq("StatusOrder", "underAnalysis")
                //Builders<NewOrder>.Filter.Ne("OrderId", orderId)  // Excluir o pedido original
            );

            var sort = Builders<NewOrder>.Sort.Descending("OrderDate");

            return await _collection.Find(similarOrdersFilter)
                                   .Sort(sort)
                                   .ToListAsync();
        }*/