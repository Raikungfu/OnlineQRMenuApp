public class OrderDto
{
    public int ProductId { get; set; }
    public int? Quantity { get; set; }  // Nullable int
    public string Size { get; set; }
    public string Option { get; set; }
    public string Note { get; set; }
    public decimal? Price { get; set; }  // Nullable decimal
}
