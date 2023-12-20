using Domain.Interfaces.IGeneric;
using Entities;

namespace Domain.Interfaces
{
    public interface IRepositoryDemand : IGenericMongoDb<Demand>
    {
        
    }
}
