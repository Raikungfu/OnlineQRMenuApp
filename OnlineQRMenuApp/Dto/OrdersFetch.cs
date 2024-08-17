namespace OnlineQRMenuApp.Dto
{
    public class OrdersFetch
    {
        public List<OrderFetchItems> Items { get; set; }
        public int OrderId { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }

    }
}
