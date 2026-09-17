namespace Application.DTOs;

public class ProductSaleRequest
{
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? DocumentNumber { get; set; }
    public int? UserId { get; set; }
}
