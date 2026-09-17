using Domain.Exceptions;

namespace Domain.Entities.ValueObjects;

public class ProductMetrics
{
    public decimal AverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public int TotalSales { get; private set; }
    public int ViewsCount { get; private set; }
    public DateTime? LastViewedAt { get; private set; }
    public DateTime? LastPurchasedAt { get; private set; }

    public ProductMetrics()
    {
        AverageRating = 0m;
        TotalReviews = 0;
        TotalSales = 0;
        ViewsCount = 0;
    }

    public void IncrementViews()
    {
        ViewsCount++;
        LastViewedAt = DateTime.UtcNow;
    }

    public void IncrementSales(int quantity = 1)
    {
        if (quantity <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.");
        
        TotalSales += quantity;
        LastPurchasedAt = DateTime.UtcNow;
    }

    public void UpdateRating(IEnumerable<ProductReview> reviews)
    {
        var reviewList = reviews.Where(r => r.IsApproved).ToList();
        TotalReviews = reviewList.Count;
        
        if (TotalReviews > 0)
        {
            // Calcular a média como double e depois converter para decimal explicitamente
            var average = reviewList.Average(r => (double)r.Rating);
            AverageRating = (decimal)Math.Round(average, 2);
        }
        else
        {
            AverageRating = 0m;
        }
    }
}