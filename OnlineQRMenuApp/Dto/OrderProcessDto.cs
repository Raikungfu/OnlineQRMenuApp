namespace OnlineQRMenuApp.Dto
{
    public class OrderProcessDto
    {
        public string status { get; set; }
        public int orderId { get; set; }
        public DateTime updateDate { get; set; }
        public DateTime orderDate { get; set; }
        public string paymentMethod { get; set; }
    }
}
