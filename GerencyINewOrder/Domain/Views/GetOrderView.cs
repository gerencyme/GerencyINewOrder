namespace Domain.Views
{
    [Serializable]
    public class GetOrderView : PaginationsControllersView
    {
        public string CompanieCNPJ { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
