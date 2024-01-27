using Domain.Interfaces.IGeneric;
using Entities.Entities;

namespace Domain.Interfaces.IRepositorys
{
    public interface IRepositoryNewOrder : IGenericMongoDb<NewOrder>
    {
        Task DeleteNewOrder(Guid id);
        Task UpdateNewOrder(NewOrder newOrder, Guid orderId);
        Task<List<NewOrder>> GetLast12NewOrders(string cnpj);
        Task<List<NewOrder>> GetOrdersByDateRangeWithPagination(string cnpj, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
        Task<List<NewOrder>> GetOrdersByDateWithPagination(string cnpj, int pageNumber, int pageSize);
    }
}
