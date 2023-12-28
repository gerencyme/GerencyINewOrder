using MongoDB.Bson;

namespace GerencyINewOrderApi.Views
{
    [Serializable]
    public class NewOrderUpdateView : NewOrderAddView
    {
        public Guid OrderId { get; set; }
    }
}
