using Entities.Entities;
using GerencyINewOrderApi.Views;
using MongoDB.Bson;

namespace Domain.Interfaces.IServices
{
    public interface INewOrderServices
    {
        Task<NewOrderUpdateView> AddNewOrder(NewOrderAddView objeto);

        Task<NewOrderUpdateView> UpdateNewOrder(NewOrderUpdateView objeto);

        Task<List<NewOrder>> ListNewOrder();

        Task<string> DeleteNewOrder(Guid idNewOrder);

        Task<NewOrder> GetByEntityId(Guid idNewOrder);


    }
}
