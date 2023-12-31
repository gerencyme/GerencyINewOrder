﻿using Domain.Interfaces.IGeneric;
using Entities.Entities;

namespace Domain.Interfaces.IRepositorys
{
    public interface IRepositoryNewOrder : IGenericMongoDb<NewOrder>
    {
        Task DeleteNewOrder(Guid id);
        Task UpdateNewOrder(NewOrder newOrder, Guid orderId);
    }
}
