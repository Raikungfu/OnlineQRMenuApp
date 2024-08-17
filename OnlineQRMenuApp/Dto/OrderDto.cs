namespace OnlineQRMenuApp.Dto
{
    public class OrderDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Option { get; set; }
        public string Note { get; set; }
        public decimal Price { get; set; }
    }
}
