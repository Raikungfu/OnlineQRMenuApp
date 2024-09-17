namespace OnlineQRMenuApp.Models.ViewModel
{
    public class OrderViewModel
    {
        public string Code { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Table { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class OrderListViewModel
    {
        public string Date { get; set; }
        public List<OrderViewModel> Children { get; set; }
    }

}
