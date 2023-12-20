using Domain.Interfaces;
using Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generic;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repository.Repositories
{
    public class ProductRepository : RepositoryMongoDBGeneric<Product>, IRepositoryProduct
    {
        public ProductRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {

        }
    }
}
