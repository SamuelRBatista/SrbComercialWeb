using Domain.Exceptions;

namespace Domain.Entities;

public class ProductAttribute
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string? Group { get; private set; } // Grupo de atributos (ex: "Especificações Técnicas", "Dimensões")
    public int? DisplayOrder { get; private set; } // Ordem de exibição
    public bool IsVisible { get; private set; } // Se é visível para o cliente
    public bool IsFilterable { get; private set; } // Se pode ser usado como filtro
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected ProductAttribute() { }

    public ProductAttribute(
        int productId,
        string key,
        string value,
        string? group = null,
        int? displayOrder = null,
        bool isVisible = true,
        bool isFilterable = false)
    {
        if (productId <= 0)
            throw new DomainException("ID do produto é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Chave do atributo é obrigatória.");
        
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Valor do atributo é obrigatório.");
        
        if (key.Length > 100)
            throw new DomainException("Chave do atributo deve ter no máximo 100 caracteres.");
        
        if (value.Length > 500)
            throw new DomainException("Valor do atributo deve ter no máximo 500 caracteres.");
        
        ProductId = productId;
        Key = key.Trim();
        Value = value.Trim();
        Group = group?.Trim();
        DisplayOrder = displayOrder;
        IsVisible = isVisible;
        IsFilterable = isFilterable;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string value, string? group = null, int? displayOrder = null, bool? isVisible = null, bool? isFilterable = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Valor do atributo é obrigatório.");
        
        if (value.Length > 500)
            throw new DomainException("Valor do atributo deve ter no máximo 500 caracteres.");
        
        Value = value.Trim();
        
        if (group != null)
            Group = group.Trim();
        
        if (displayOrder.HasValue)
            DisplayOrder = displayOrder;
        
        if (isVisible.HasValue)
            IsVisible = isVisible.Value;
        
        if (isFilterable.HasValue)
            IsFilterable = isFilterable.Value;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetVisibility(bool isVisible)
    {
        IsVisible = isVisible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFilterable(bool isFilterable)
    {
        IsFilterable = isFilterable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 0)
            throw new DomainException("Ordem de exibição não pode ser negativa.");
        
        DisplayOrder = order;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{Key}: {Value}";
    }
}