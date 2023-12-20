using Domain.Interfaces;
using Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Repository.Repositories
{
    public class DemandRepository : RepositoryMongoDBGeneric<Demand>, IRepositoryDemand
    {
        private readonly IMongoCollection<Demand> _collection;

        public DemandRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

    }
}
