namespace Domain.Views
{
    [Serializable]
    public class PaginationsControllersView
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
