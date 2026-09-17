namespace Domain.Entities;

public class ReviewHelpful
{
    public int Id { get; private set; }
    public int ReviewId { get; private set; }
    public ProductReview? Review { get; private set; }
    public int UserId { get; private set; }
    public bool IsHelpful { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected ReviewHelpful() { }

    public ReviewHelpful(int reviewId, int userId, bool isHelpful)
    {
        ReviewId = reviewId;
        UserId = userId;
        IsHelpful = isHelpful;
        CreatedAt = DateTime.UtcNow;
    }
}