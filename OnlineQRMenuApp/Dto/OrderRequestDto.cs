namespace OnlineQRMenuApp.Dto
{
    public class OrderRequestDto
    {
        public List<OrderDto> Items { get; set; }
        public string PaymentMethod { get; set; }
    }
}
