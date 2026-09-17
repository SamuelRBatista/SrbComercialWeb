using Domain.Exceptions;

namespace Domain.Entities.ValueObjects;

public class PriceInfo
{
    public decimal SalePrice { get; private set; }
    public decimal CostPrice { get; private set; }
    public decimal? WholesalePrice { get; private set; }
    public int? WholesaleMinQuantity { get; private set; }
    public decimal? PromotionalPrice { get; private set; }
    public DateTime? PromotionStartDate { get; private set; }
    public DateTime? PromotionEndDate { get; private set; }
    public int? PromotionId { get; private set; }

    public PriceInfo(decimal salePrice, decimal costPrice)
    {
        ValidatePrices(salePrice, costPrice);
        SalePrice = salePrice;
        CostPrice = costPrice;
    }

    // ADICIONAR ESTE MÉTODO
    public void SetCostPrice(decimal costPrice)
    {
        if (costPrice <= 0)
            throw new DomainException("Preço de custo deve ser maior que zero.");
        
        if (costPrice > SalePrice)
            throw new DomainException("Preço de custo não pode ser maior que o preço de venda.");
        
        CostPrice = costPrice;
    }

    public void UpdateSalePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainException("Preço de venda deve ser maior que zero.");
        
        if (newPrice < CostPrice)
            throw new DomainException("Preço de venda não pode ser menor que o preço de custo.");
        
        SalePrice = newPrice;
    }

    public void UpdateCostPrice(decimal newCostPrice)
    {
        if (newCostPrice <= 0)
            throw new DomainException("Preço de custo deve ser maior que zero.");
        
        if (newCostPrice > SalePrice)
            throw new DomainException("Preço de custo não pode ser maior que o preço de venda.");
        
        CostPrice = newCostPrice;
    }

    public void SetWholesalePrice(decimal wholesalePrice, int minQuantity)
    {
        if (wholesalePrice <= 0)
            throw new DomainException("Preço de atacado deve ser maior que zero.");
        
        if (minQuantity <= 0)
            throw new DomainException("Quantidade mínima para atacado deve ser maior que zero.");
        
        if (wholesalePrice >= SalePrice)
            throw new DomainException("Preço de atacado deve ser menor que o preço normal.");
        
        WholesalePrice = wholesalePrice;
        WholesaleMinQuantity = minQuantity;
    }

    public void ApplyPromotion(decimal promotionalPrice, DateTime startDate, DateTime endDate, int? promotionId = null)
    {
        if (promotionalPrice <= 0)
            throw new DomainException("Preço promocional deve ser maior que zero.");
        
        if (promotionalPrice >= SalePrice)
            throw new DomainException("Preço promocional deve ser menor que o preço normal.");
        
        if (promotionalPrice < CostPrice)
            throw new DomainException("Preço promocional não pode ser menor que o preço de custo.");
        
        if (startDate >= endDate)
            throw new DomainException("Data de início deve ser anterior à data de fim.");
        
        PromotionalPrice = promotionalPrice;
        PromotionStartDate = startDate;
        PromotionEndDate = endDate;
        PromotionId = promotionId;
    }

    public void RemovePromotion()
    {
        PromotionalPrice = null;
        PromotionStartDate = null;
        PromotionEndDate = null;
        PromotionId = null;
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
        return HasPromotion() ? PromotionalPrice.Value : SalePrice;
    }

    public decimal GetDiscountPercentage()
    {
        if (!HasPromotion())
            return 0;
        
        return ((SalePrice - PromotionalPrice.Value) / SalePrice) * 100;
    }

    public decimal GetPriceForQuantity(int quantity)
    {
        if (WholesalePrice.HasValue && WholesaleMinQuantity.HasValue && 
            quantity >= WholesaleMinQuantity.Value)
        {
            return WholesalePrice.Value;
        }
        
        return GetCurrentPrice();
    }

    private void ValidatePrices(decimal salePrice, decimal costPrice)
    {
        if (salePrice <= 0)
            throw new DomainException("Preço de venda deve ser maior que zero.");
        
        if (costPrice <= 0)
            throw new DomainException("Preço de custo deve ser maior que zero.");
        
        if (costPrice > salePrice)
            throw new DomainException("Preço de custo não pode ser maior que o preço de venda.");
    }
}