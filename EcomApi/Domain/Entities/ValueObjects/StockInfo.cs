using Domain.Exceptions;

namespace Domain.Entities.ValueObjects;

public class StockInfo
{
    public int Quantity { get; private set; }
    public int? MinimumStock { get; private set; }
    public int? MaximumStock { get; private set; }
    public string UnitOfMeasure { get; private set; }
    public bool IsTrackInventory { get; private set; }
    public bool AllowBackorder { get; private set; }
    public DateTime? ExpectedStockDate { get; private set; }

    public StockInfo(
        int quantity, 
        string unitOfMeasure = "UN",
        int? minimumStock = null,
        int? maximumStock = null,
        bool isTrackInventory = true,
        bool allowBackorder = false)
    {
        if (quantity < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");
        
        if (string.IsNullOrWhiteSpace(unitOfMeasure))
            throw new DomainException("Unidade de medida é obrigatória.");
        
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        MinimumStock = minimumStock;
        MaximumStock = maximumStock;
        IsTrackInventory = isTrackInventory;
        AllowBackorder = allowBackorder;
    }

    // ADICIONAR ESTE MÉTODO
    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Quantidade em estoque não pode ser negativa.");
        
        if (MaximumStock.HasValue && quantity > MaximumStock.Value)
            throw new DomainException($"Quantidade excede o estoque máximo de {MaximumStock.Value}.");
        
        Quantity = quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade para adicionar deve ser maior que zero.");
        
        if (MaximumStock.HasValue && Quantity + quantity > MaximumStock.Value)
            throw new DomainException($"Quantidade excede o estoque máximo de {MaximumStock.Value}.");
        
        Quantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade para remover deve ser maior que zero.");
        
        if (!AllowBackorder && Quantity - quantity < 0)
            throw new DomainException($"Estoque insuficiente. Disponível: {Quantity}, Solicitado: {quantity}");
        
        Quantity = Math.Max(0, Quantity - quantity);
    }

    public void AdjustStock(int newQuantity)
    {
        if (newQuantity < 0)
            throw new DomainException("Quantidade não pode ser negativa.");
        
        if (MaximumStock.HasValue && newQuantity > MaximumStock.Value)
            throw new DomainException($"Quantidade excede o estoque máximo de {MaximumStock.Value}.");
        
        Quantity = newQuantity;
    }

    public bool IsAvailable()
    {
        if (!IsTrackInventory)
            return true;
        
        return Quantity > 0 || AllowBackorder;
    }

    public bool IsAvailableQuantity(int quantity)
    {
        if (!IsAvailable())
            return false;
        
        if (!IsTrackInventory)
            return true;
        
        if (AllowBackorder)
            return true;
        
        return Quantity >= quantity;
    }

    public bool IsLowStock()
    {
        return MinimumStock.HasValue && Quantity <= MinimumStock.Value;
    }

    public bool IsOutOfStock()
    {
        return Quantity <= 0;
    }

    public void SetMinimumStock(int minimum)
    {
        if (minimum < 0)
            throw new DomainException("Estoque mínimo não pode ser negativo.");
        
        if (MaximumStock.HasValue && minimum > MaximumStock.Value)
            throw new DomainException("Estoque mínimo não pode ser maior que o máximo.");
        
        MinimumStock = minimum;
    }

    public void SetMaximumStock(int maximum)
    {
        if (maximum < 0)
            throw new DomainException("Estoque máximo não pode ser negativo.");
        
        if (MinimumStock.HasValue && maximum < MinimumStock.Value)
            throw new DomainException("Estoque máximo não pode ser menor que o mínimo.");
        
        MaximumStock = maximum;
    }

    public void SetExpectedStockDate(DateTime? date)
    {
        if (date.HasValue && date.Value < DateTime.UtcNow)
            throw new DomainException("Data prevista não pode ser no passado.");
        
        ExpectedStockDate = date;
    }
}