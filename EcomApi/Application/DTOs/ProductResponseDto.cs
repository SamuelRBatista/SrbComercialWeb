namespace Application.DTOs;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CostPrice { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string BarCode { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? SupplierId { get; set; }
    public int StockQuantity { get; set; }
    public int? MinimumStock { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Width { get; set; }
    public decimal? Depth { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsDigital { get; set; }
    public bool HasVariants { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalSales { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Observations { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductAttributeDto> Attributes { get; set; } = new();
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
}

public class ProductAttributeDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Group { get; set; }
    public int? DisplayOrder { get; set; }
}