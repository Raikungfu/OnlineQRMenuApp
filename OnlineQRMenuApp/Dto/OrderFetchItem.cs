namespace OnlineQRMenuApp.Dto
{
    public class OrderFetchItems
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Option { get; set; }
        public string Note { get; set; }
        public decimal? Price { get; set; } 

    }
}
