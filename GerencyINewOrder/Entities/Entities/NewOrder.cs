﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Entities
{
    [Serializable]
    public class NewOrder
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanieCNPJ { get; set; }
        public string OrderColorIdentity { get; set; }
        public DateTime OrderDate { get; set; }
        public List<Product> Product { get; set; }
        public Location Location { get; set; }

        public NewOrder()
        {
            Product = new List<Product>();
            Location = new Location();
            OrderId = Guid.NewGuid();
            Id = ObjectId.GenerateNewId();
        }
    }
}