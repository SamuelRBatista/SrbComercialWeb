using Domain.Entities.ValueObjects;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Product
{
    // Propriedades principais
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ShortDescription { get; private set; }
    public string Sku { get; private set; }
    public string BarCode { get; private set; }
    public string MainImageUrl { get; private set; }
    
    // Preços
    public PriceInfo PriceInfo { get; private set; }
    
    // Estoque
    public StockInfo StockInfo { get; private set; }
    
    // Dimensões
    public Dimensions Dimensions { get; private set; }
    
    // Especificações
    public ProductSpecs Specs { get; private set; }
    
    // SEO
    public SeoInfo SeoInfo { get; private set; }
    
    // Relacionamentos
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    
    public int? SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }
    
    // Status
    public ProductStatus Status { get; private set; }
    public VisibilityStatus Visibility { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsNew { get; private set; }
    public bool IsDigital { get; private set; }
    public bool HasVariants { get; private set; } // Indica se o produto tem variações
    
    // Métricas
    public ProductMetrics Metrics { get; private set; }
    
    // Auditoria
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public int? CreatedByUserId { get; private set; }
    public int? UpdatedByUserId { get; private set; }
    
    // Observações
    public string Observations { get; private set; }
    public string InternalNotes { get; private set; }
    
    // Coleções
    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    
    private readonly List<ProductVariant> _variants = new();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    
    private readonly List<StockMovement> _stockMovements = new();
    public IReadOnlyCollection<StockMovement> StockMovements => _stockMovements.AsReadOnly();
    
    private readonly List<ProductAttribute> _attributes = new();
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();
    
    private readonly List<ProductReview> _reviews = new();
    public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();

    protected Product() { }

    // Construtor para produto SEM variações
    public Product(
        string name,
        string description,
        string sku,
        decimal price,
        int categoryId,
        int stockQuantity = 0,
        string shortDescription = null,
        string barCode = null,
        string mainImageUrl = null,
        int? supplierId = null,
        decimal? costPrice = null,
        string unitOfMeasure = "UN",
        decimal? weight = null,
        decimal? height = null,
        decimal? width = null,
        decimal? depth = null,
        string brand = null,
        string model = null,
        bool isDigital = false,
        int? createdByUserId = null)
    {
        ValidateBasicFields(name, description, sku, price, categoryId);
        
        Name = name;
        Description = description;
        ShortDescription = shortDescription ?? TruncateDescription(description, 150);
        Sku = sku;
        BarCode = barCode;
        MainImageUrl = mainImageUrl;
        CategoryId = categoryId;
        SupplierId = supplierId;
        IsDigital = isDigital;
        CreatedByUserId = createdByUserId;
        HasVariants = false;        
        PriceInfo = new PriceInfo(price, costPrice ?? price * 0.6m);
        StockInfo = new StockInfo(stockQuantity, unitOfMeasure);
        Dimensions = new Dimensions(weight, height, width, depth);
        Specs = new ProductSpecs(brand, model);
        SeoInfo = new SeoInfo(name);
        Metrics = new ProductMetrics();        
        Status = ProductStatus.Draft;
        Visibility = VisibilityStatus.NotVisible;
        IsActive = false;
        IsFeatured = false;
        IsNew = true;        
        CreatedAt = DateTime.UtcNow;
        SeoInfo.GenerateSlug(name);
    }

    // Construtor para produto COM variações
    public Product(
        string name,
        string description,
        string sku,
        int categoryId,
        string shortDescription = null,
        string mainImageUrl = null,
        int? supplierId = null,
        string brand = null,
        string model = null,
        bool isDigital = false,
        int? createdByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do produto é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU é obrigatório.");
        
        if (categoryId <= 0)
            throw new DomainException("Categoria é obrigatória.");
        
        Name = name;
        Description = description;
        ShortDescription = shortDescription ?? TruncateDescription(description, 150);
        Sku = sku;
        MainImageUrl = mainImageUrl;
        CategoryId = categoryId;
        SupplierId = supplierId;
        IsDigital = isDigital;
        CreatedByUserId = createdByUserId;
        HasVariants = true;
        
        PriceInfo = new PriceInfo(0, 0);
        StockInfo = new StockInfo(0);
        Dimensions = new Dimensions();
        Specs = new ProductSpecs(brand, model);
        SeoInfo = new SeoInfo(name);
        Metrics = new ProductMetrics();
        
        Status = ProductStatus.Draft;
        Visibility = VisibilityStatus.NotVisible;
        IsActive = false;
        IsFeatured = false;
        IsNew = true;
        
        CreatedAt = DateTime.UtcNow;
        SeoInfo.GenerateSlug(name);
    }

    // Métodos de variações
    public void AddVariant(
        string name,
        string sku,
        decimal price,
        int stockQuantity,
        Dictionary<string, string> attributes,
        string? imageUrl = null,
        string? barCode = null,
        decimal? costPrice = null,
        int? minimumStock = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da variante é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU da variante é obrigatório.");
        
        if (price <= 0)
            throw new DomainException("Preço da variante deve ser maior que zero.");
        
        if (stockQuantity < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");
        
        // Verificar se SKU já existe
        if (_variants.Any(v => v.Sku == sku))
            throw new DomainException($"Já existe uma variante com o SKU '{sku}'.");
        
        var variant = new ProductVariant(
            productId: Id,
            name: name,
            sku: sku,
            price: price,
            stockQuantity: stockQuantity,
            imageUrl: imageUrl,
            barCode: barCode,
            costPrice: costPrice ?? price * 0.6m,
            minimumStock: minimumStock
        );
        
        // Adicionar atributos da variação
        foreach (var attr in attributes)
        {
            variant.AddAttribute(attr.Key, attr.Value);
        }
        
        _variants.Add(variant);
        HasVariants = true;
        UpdatedAt = DateTime.UtcNow;
        
        // Atualizar preço do produto base com o menor preço das variações
        UpdateBasePriceFromVariants();
    }

    public void UpdateVariant(
        int variantId,
        string name,
        string sku,
        decimal price,
        int stockQuantity,
        string? imageUrl = null,
        string? barCode = null,
        decimal? costPrice = null,
        int? minimumStock = null)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new DomainException("Variante não encontrada.");
        
        variant.Update(name, sku, price, costPrice ?? price * 0.6m, imageUrl, barCode, minimumStock);
        variant.UpdateStock(stockQuantity);
        
        UpdatedAt = DateTime.UtcNow;
        UpdateBasePriceFromVariants();
    }

    public void RemoveVariant(int variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new DomainException("Variante não encontrada.");
        
        _variants.Remove(variant);
        UpdatedAt = DateTime.UtcNow;
        
        if (!_variants.Any())
        {
            HasVariants = false;
        }
        else
        {
            UpdateBasePriceFromVariants();
        }
    }

    public void UpdateVariantStock(int variantId, int quantity)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new DomainException("Variante não encontrada.");
        
        variant.UpdateStock(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddVariantStock(int variantId, int quantity, string reason, string? documentNumber = null, int? userId = null)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new DomainException("Variante não encontrada.");
        
        variant.AddStock(quantity);
        
        var movement = new StockMovement(
            productId: Id,
            variantId: variantId,
            quantity: quantity,
            type: StockMovementType.Inbound,
            reason: reason,
            documentNumber: documentNumber,
            userId: userId
        );
        
        _stockMovements.Add(movement);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveVariantStock(int variantId, int quantity, string reason, string? documentNumber = null, int? userId = null)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new DomainException("Variante não encontrada.");
        
        variant.RemoveStock(quantity);
        
        var movement = new StockMovement(
            productId: Id,
            variantId: variantId,
            quantity: -quantity,
            type: StockMovementType.Outbound,
            reason: reason,
            documentNumber: documentNumber,
            userId: userId
        );
        
        _stockMovements.Add(movement);
        UpdatedAt = DateTime.UtcNow;
    }

    public ProductVariant? GetVariantBySku(string sku)
    {
        return _variants.FirstOrDefault(v => v.Sku == sku);
    }

    public ProductVariant? GetVariantById(int variantId)
    {
        return _variants.FirstOrDefault(v => v.Id == variantId);
    }

    public IEnumerable<ProductVariant> GetActiveVariants()
    {
        return _variants.Where(v => v.IsActive);
    }

    public IEnumerable<ProductVariant> GetVariantsInStock()
    {
        return _variants.Where(v => v.IsInStock());
    }

    public int GetTotalStock()
    {
        if (!HasVariants)
            return StockInfo.Quantity;
        
        return _variants.Sum(v => v.StockQuantity);
    }

    public decimal GetLowestPrice()
    {
        if (!HasVariants)
            return PriceInfo.GetCurrentPrice();
        
        return _variants.Min(v => v.GetCurrentPrice());
    }

    public decimal GetHighestPrice()
    {
        if (!HasVariants)
            return PriceInfo.GetCurrentPrice();
        
        return _variants.Max(v => v.GetCurrentPrice());
    }

    public string GetPriceRange()
    {
        if (!HasVariants)
            return $"R$ {GetCurrentPrice():F2}";
        
        var lowest = GetLowestPrice();
        var highest = GetHighestPrice();
        
        if (lowest == highest)
            return $"R$ {lowest:F2}";
        
        return $"R$ {lowest:F2} - R$ {highest:F2}";
    }

    private void UpdateBasePriceFromVariants()
    {
        if (!_variants.Any())
            return;
        
        var lowestPrice = _variants.Min(v => v.Price);
        PriceInfo.UpdateSalePrice(lowestPrice);
    }

    // Métodos de estoque para produto SEM variações
    public void AddStock(int quantity, string reason, string documentNumber = null, int? userId = null)
    {
        if (HasVariants)
            throw new DomainException("Este produto tem variações. Use os métodos específicos para variações.");
        
        StockInfo.AddStock(quantity);
        
        var movement = new StockMovement(
            productId: Id,
            quantity: quantity,
            type: StockMovementType.Inbound,
            reason: reason,
            documentNumber: documentNumber,
            userId: userId
        );
        
        _stockMovements.Add(movement);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity, string reason, string documentNumber = null, int? userId = null)
    {
        if (HasVariants)
            throw new DomainException("Este produto tem variações. Use os métodos específicos para variações.");
        
        StockInfo.RemoveStock(quantity);
        
        var movement = new StockMovement(
            productId: Id,
            quantity: -quantity,
            type: StockMovementType.Outbound,
            reason: reason,
            documentNumber: documentNumber,
            userId: userId
        );
        
        _stockMovements.Add(movement);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable()
    {
        if (!IsActive || Status != ProductStatus.Active)
            return false;
        
        if (HasVariants)
            return _variants.Any(v => v.IsAvailable());
        
        return StockInfo.IsAvailable();
    }

    public bool IsAvailableQuantity(int quantity)
    {
        if (!IsAvailable())
            return false;
        
        if (HasVariants)
            return _variants.Sum(v => v.StockQuantity) >= quantity;
        
        return StockInfo.IsAvailableQuantity(quantity);
    }

    // Métodos de preço
    public void UpdatePrice(decimal newPrice, int? updatedByUserId = null)
    {
        if (HasVariants)
            throw new DomainException("Este produto tem variações. O preço é definido por variação.");
        
        PriceInfo.UpdateSalePrice(newPrice);
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyPromotion(decimal promotionalPrice, DateTime startDate, DateTime endDate, int? promotionId = null)
    {
        if (HasVariants)
        {
            // Aplicar promoção em todas as variações
            foreach (var variant in _variants)
            {
                variant.ApplyPromotion(promotionalPrice, startDate, endDate, promotionId);
            }
        }
        else
        {
            PriceInfo.ApplyPromotion(promotionalPrice, startDate, endDate, promotionId);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePromotion()
    {
        if (HasVariants)
        {
            foreach (var variant in _variants)
            {
                variant.RemovePromotion();
            }
        }
        else
        {
            PriceInfo.RemovePromotion();
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetCurrentPrice()
    {
        if (HasVariants)
            return GetLowestPrice();
        
        return PriceInfo.GetCurrentPrice();
    }

    public bool HasPromotion()
    {
        if (HasVariants)
            return _variants.Any(v => v.HasPromotion());
        
        return PriceInfo.HasPromotion();
    }

    // Métodos de imagens
    public void AddImage(string imageUrl, bool isMain = false, string description = null, int order = 0)
    {
        if (_images.Any(i => i.Url == imageUrl))
            throw new DomainException("Imagem já adicionada ao produto.");
        
        var image = new ProductImage(Id, imageUrl, isMain, description, order);
        _images.Add(image);
        
        if (isMain)
            MainImageUrl = imageUrl;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveImage(int imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new DomainException("Imagem não encontrada.");
        
        if (image.IsMain)
        {
            var newMain = _images.FirstOrDefault(i => i.Id != imageId);
            if (newMain != null)
            {
                newMain.SetMain(true);
                MainImageUrl = newMain.Url;
            }
            else
            {
                MainImageUrl = null;
            }
        }
        
        _images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    // Métodos de atributos
    public void AddAttribute(string key, string value, string group = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Chave do atributo é obrigatória.");
        
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Valor do atributo é obrigatório.");
        
        var attribute = new ProductAttribute(Id, key, value, group);
        _attributes.Add(attribute);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAttribute(int attributeId)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Id == attributeId);
        if (attribute == null)
            throw new DomainException("Atributo não encontrado.");
        
        _attributes.Remove(attribute);
        UpdatedAt = DateTime.UtcNow;
    }

    // Métodos de reviews
    public void AddReview(int rating, string comment, int userId, string userName = null)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Avaliação deve ser entre 1 e 5 estrelas.");
        
        var review = new ProductReview(Id, userId, rating, comment, userName);
        _reviews.Add(review);
        
        Metrics.UpdateRating(_reviews);
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementViews()
    {
        Metrics.IncrementViews();
        UpdatedAt = DateTime.UtcNow;
    }

    // Métodos de status
    public void Publish()
    {
        ValidateBeforePublish();
        Status = ProductStatus.Active;
        IsActive = true;
        Visibility = VisibilityStatus.Visible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = ProductStatus.Inactive;
        IsActive = false;
        Visibility = VisibilityStatus.NotVisible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFeatured()
    {
        IsFeatured = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnmarkAsFeatured()
    {
        IsFeatured = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Métodos de relacionamento
    public void SetCategory(Category category)
    {
        Category = category;
        CategoryId = category.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSupplier(Supplier supplier)
    {
        Supplier = supplier;
        SupplierId = supplier.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetId(int id)
    {
        if (id <= 0)
            throw new DomainException("Id inválido.");
        
        Id = id;
        SeoInfo.GenerateSlug(Name, id);
    }

    // Métodos privados
    private void ValidateBasicFields(string name, string description, string sku, decimal price, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do produto é obrigatório.");
        
        if (name.Length < 3 || name.Length > 200)
            throw new DomainException("Nome deve ter entre 3 e 200 caracteres.");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU é obrigatório.");
        
        if (sku.Length > 50)
            throw new DomainException("SKU deve ter no máximo 50 caracteres.");
        
        if (price <= 0)
            throw new DomainException("Preço deve ser maior que zero.");
        
        if (categoryId <= 0)
            throw new DomainException("Categoria é obrigatória.");
    }

    private void ValidateBeforePublish()
    {
        if (string.IsNullOrWhiteSpace(Sku))
            throw new DomainException("Não é possível publicar produto sem SKU.");
        
        if (CategoryId <= 0)
            throw new DomainException("Não é possível publicar produto sem categoria.");
        
        if (string.IsNullOrWhiteSpace(MainImageUrl))
            throw new DomainException("Não é possível publicar produto sem imagem principal.");
        
        if (HasVariants && !_variants.Any())
            throw new DomainException("Produto com variações deve ter pelo menos uma variação.");
        
        if (!HasVariants && PriceInfo.SalePrice <= 0)
            throw new DomainException("Não é possível publicar produto sem preço definido.");
    }

    private string TruncateDescription(string description, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;
        
        return description.Length <= maxLength 
            ? description 
            : description.Substring(0, maxLength) + "...";
    }
}