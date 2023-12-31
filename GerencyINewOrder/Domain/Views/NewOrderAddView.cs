﻿using Entities.Entities;
using Entities;
using MongoDB.Bson;

namespace GerencyINewOrderApi.Views
{
    [Serializable]
    public class NewOrderAddView
    {
        public Guid CompanyId { get; set; }
        public string CompanieCNPJ { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderColorIdentity { get; set; }
        public List<Product> Product { get; set; }
        public Location Location { get; set; }
    }
}
