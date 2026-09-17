using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public class ProductCreateRequest
{
    // Propriedades obrigatórias
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    
    // Propriedades opcionais
    public string? ShortDescription { get; set; }
    public string? BarCode { get; set; }
    public int? SupplierId { get; set; }
    public decimal? CostPrice { get; set; }
    public int? StockQuantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Width { get; set; }
    public decimal? Depth { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public bool? IsDigital { get; set; }
    
    // Upload de arquivos
    public IFormFile? Image { get; set; }
    public List<IFormFile>? AdditionalImages { get; set; }
    
    // Atributos personalizados
    public List<ProductAttributeRequest>? Attributes { get; set; }
}

public class ProductAttributeRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Group { get; set; }
}