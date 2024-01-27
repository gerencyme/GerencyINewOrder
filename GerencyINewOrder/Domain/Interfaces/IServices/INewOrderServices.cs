using Domain.Views;
using Entities.Entities;
using GerencyINewOrderApi.Views;
using MongoDB.Bson;

namespace Domain.Interfaces.IServices
{
    public interface INewOrderServices
    {
        Task<NewOrderUpdateView> AddNewOrder(NewOrderAddView objeto);

        Task<NewOrderUpdateView> UpdateNewOrder(NewOrderUpdateView objeto);

        Task<string> DeleteNewOrder(Guid idNewOrder);

        Task<NewOrder> GetByEntityId(Guid idNewOrder);

        Task<List<NewOrder>> ListNewOrder();

        Task<List<OrderCardView>> GetLast12NewOrders(string cnpj);

        Task<List<NewOrder>> GetOrdersByDateRangeWithPagination(GetOrderView paginatiionNeworder);

        Task<List<OrderCardView>> GetOrdersByDateWithPagination(GetOrderView paginatiionNeworder);

    }
}
