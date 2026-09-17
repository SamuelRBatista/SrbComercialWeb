using Domain.Exceptions;

namespace Domain.Entities;

public class ProductImage
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Url { get; private set; }
    public bool IsMain { get; private set; }
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected ProductImage() 
    {
        Url = string.Empty;
    }

    public ProductImage(
        int productId,
        string url,
        bool isMain = false,
        string? description = null,
        int order = 0)
    {
        if (productId <= 0)
            throw new DomainException("ID do produto é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL da imagem é obrigatória.");
        
        ProductId = productId;
        Url = url;
        IsMain = isMain;
        Description = description;
        Order = order;
        CreatedAt = DateTime.UtcNow;
    }

    // ADICIONAR ESTE MÉTODO
    public void SetMain(bool isMain)
    {
        IsMain = isMain;
    }

    public void Update(string url, string? description, int order)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL da imagem é obrigatória.");
        
        Url = url;
        Description = description;
        Order = order;
    }
}