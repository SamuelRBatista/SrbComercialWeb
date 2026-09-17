using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class StockMovement
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int? VariantId { get; private set; } // Para movimentação de variação
    public ProductVariant? Variant { get; private set; }
    public int Quantity { get; private set; }
    public StockMovementType Type { get; private set; }
    public string Reason { get; private set; }
    public string? DocumentNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public decimal? UnitCost { get; private set; } // Custo unitário no momento da movimentação
    public decimal? TotalCost { get; private set; } // Custo total da movimentação
    public int? PreviousStock { get; private set; } // Estoque antes da movimentação
    public int? NewStock { get; private set; } // Estoque após a movimentação
    public string? Notes { get; private set; }

    protected StockMovement() { }

    // Construtor para movimentação de produto (sem variação)
    public StockMovement(
        int productId,
        int quantity,
        StockMovementType type,
        string reason,
        string? documentNumber = null,
        int? userId = null,
        string? userName = null,
        decimal? unitCost = null,
        int? previousStock = null,
        int? newStock = null,
        string? notes = null)
    {
        if (productId <= 0)
            throw new DomainException("ID do produto é obrigatório.");
        
        if (quantity == 0)
            throw new DomainException("Quantidade não pode ser zero.");
        
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Motivo do movimento é obrigatório.");
        
        ProductId = productId;
        Quantity = quantity;
        Type = type;
        Reason = reason;
        DocumentNumber = documentNumber;
        UserId = userId;
        UserName = userName;
        UnitCost = unitCost;
        TotalCost = unitCost.HasValue ? unitCost.Value * Math.Abs(quantity) : null;
        PreviousStock = previousStock;
        NewStock = newStock;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    // Construtor para movimentação de variação
    public StockMovement(
        int productId,
        int variantId,
        int quantity,
        StockMovementType type,
        string reason,
        string? documentNumber = null,
        int? userId = null,
        string? userName = null,
        decimal? unitCost = null,
        int? previousStock = null,
        int? newStock = null,
        string? notes = null)
        : this(productId, quantity, type, reason, documentNumber, userId, userName, unitCost, previousStock, newStock, notes)
    {
        if (variantId <= 0)
            throw new DomainException("ID da variação é obrigatório.");
        
        VariantId = variantId;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
    }

    public string GetMovementDescription()
    {
        var typeDescription = Type switch
        {
            StockMovementType.Inbound => "Entrada",
            StockMovementType.Outbound => "Saída",
            StockMovementType.Adjustment => "Ajuste",
            StockMovementType.Return => "Devolução",
            StockMovementType.Transfer => "Transferência",
            StockMovementType.Loss => "Perda",
            StockMovementType.Damage => "Danificado",
            StockMovementType.Reservation => "Reserva",
            StockMovementType.Cancellation => "Cancelamento",
            _ => "Desconhecido"
        };

        var direction = Quantity > 0 ? "+" : "";
        var variantInfo = VariantId.HasValue ? $" (Variação {VariantId})" : "";
        
        return $"{typeDescription}{variantInfo}: {direction}{Quantity} unidades - {Reason}";
    }

    public bool IsEntry() => Quantity > 0 && 
        (Type == StockMovementType.Inbound || 
         Type == StockMovementType.Return || 
         Type == StockMovementType.Adjustment);

    public bool IsExit() => Quantity < 0 && 
        (Type == StockMovementType.Outbound || 
         Type == StockMovementType.Loss || 
         Type == StockMovementType.Damage || 
         Type == StockMovementType.Transfer);
}