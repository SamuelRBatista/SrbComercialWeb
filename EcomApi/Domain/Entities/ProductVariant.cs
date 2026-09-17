using Domain.Entities.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities;

public class ProductVariant
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public string? BarCode { get; private set; }
    public decimal Price { get; private set; }
    public decimal CostPrice { get; private set; }
    public int StockQuantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int? MinimumStock { get; private set; }
    
    // Preço promocional por variação
    public decimal? PromotionalPrice { get; private set; }
    public DateTime? PromotionStartDate { get; private set; }
    public DateTime? PromotionEndDate { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<VariantAttribute> _attributes = new();
    public IReadOnlyCollection<VariantAttribute> Attributes => _attributes.AsReadOnly();

    protected ProductVariant() { }

    public ProductVariant(
        int productId,
        string name,
        string sku,
        decimal price,
        int stockQuantity,
        string? imageUrl = null,
        string? barCode = null,
        decimal? costPrice = null,
        int? minimumStock = null)
    {
        if (productId <= 0)
            throw new DomainException("ID do produto é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da variante é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU da variante é obrigatório.");
        
        if (price <= 0)
            throw new DomainException("Preço da variante deve ser maior que zero.");
        
        if (stockQuantity < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");
        
        ProductId = productId;
        Name = name;
        Sku = sku;
        Price = price;
        CostPrice = costPrice ?? price * 0.6m;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl;
        BarCode = barCode;
        MinimumStock = minimumStock;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string sku,
        decimal price,
        decimal costPrice,
        string? imageUrl = null,
        string? barCode = null,
        int? minimumStock = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da variante é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU da variante é obrigatório.");
        
        if (price <= 0)
            throw new DomainException("Preço da variante deve ser maior que zero.");
        
        if (costPrice <= 0)
            throw new DomainException("Preço de custo da variante deve ser maior que zero.");
        
        Name = name;
        Sku = sku;
        Price = price;
        CostPrice = costPrice;
        ImageUrl = imageUrl;
        BarCode = barCode;
        MinimumStock = minimumStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAttribute(string key, string value)
    {
        var attribute = new VariantAttribute(Id, key, value);
        _attributes.Add(attribute);
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");
        
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.");
        
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.");
        
        if (StockQuantity - quantity < 0)
            throw new DomainException($"Estoque insuficiente. Disponível: {StockQuantity}");
        
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyPromotion(decimal promotionalPrice, DateTime startDate, DateTime endDate, int? promotionId = null)
    {
        if (promotionalPrice <= 0)
            throw new DomainException("Preço promocional deve ser maior que zero.");
        
        if (promotionalPrice >= Price)
            throw new DomainException("Preço promocional deve ser menor que o preço normal.");
        
        if (promotionalPrice < CostPrice)
            throw new DomainException("Preço promocional não pode ser menor que o preço de custo.");
        
        if (startDate >= endDate)
            throw new DomainException("Data de início deve ser anterior à data de fim.");
        
        PromotionalPrice = promotionalPrice;
        PromotionStartDate = startDate;
        PromotionEndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePromotion()
    {
        PromotionalPrice = null;
        PromotionStartDate = null;
        PromotionEndDate = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasPromotion()
    {
        return PromotionalPrice.HasValue && 
               PromotionStartDate.HasValue && 
               PromotionEndDate.HasValue &&
               PromotionStartDate <= DateTime.UtcNow && 
               PromotionEndDate >= DateTime.UtcNow;
    }

    public decimal GetCurrentPrice()
    {
        return HasPromotion() ? PromotionalPrice.Value : Price;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsInStock() => StockQuantity > 0;
    public bool IsLowStock() => MinimumStock.HasValue && StockQuantity <= MinimumStock.Value;
    public bool IsAvailable() => IsActive && (StockQuantity > 0);
}