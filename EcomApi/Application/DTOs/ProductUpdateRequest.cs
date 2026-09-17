using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public class ProductUpdateRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? BarCode { get; set; }
    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }
    
    // Preços (opcionais para atualização parcial)
    public decimal? Price { get; set; }
    public decimal? CostPrice { get; set; }
    
    // Estoque
    public int? StockQuantity { get; set; }
    
    // Status
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    
    // Upload de arquivos
    public IFormFile? Image { get; set; }
    public string? ExistingImageUrl { get; set; }
}