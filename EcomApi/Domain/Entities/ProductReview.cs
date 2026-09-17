using Domain.Exceptions;

namespace Domain.Entities;

public class ProductReview
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }
    public int Rating { get; private set; }
    public string? Title { get; private set; }
    public string Comment { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsVerifiedPurchase { get; private set; }
    public bool IsRecommended { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public int HelpfulCount { get; private set; }
    public int NotHelpfulCount { get; private set; }
    public string? AdminResponse { get; private set; }
    public DateTime? AdminResponseAt { get; private set; }
    public string? Images { get; private set; }

    private readonly List<ReviewHelpful> _helpfulVotes = new();
    public IReadOnlyCollection<ReviewHelpful> HelpfulVotes => _helpfulVotes.AsReadOnly();

    protected ProductReview() 
    {
        Comment = string.Empty;
    }

    public ProductReview(
        int productId,
        int userId,
        int rating,
        string comment,
        string? title = null,
        string? userName = null,
        string? userEmail = null,
        bool isVerifiedPurchase = false,
        bool isRecommended = true,
        string? images = null)
    {
        if (productId <= 0)
            throw new DomainException("ID do produto é obrigatório.");
        
        if (userId <= 0)
            throw new DomainException("ID do usuário é obrigatório.");
        
        if (rating < 1 || rating > 5)
            throw new DomainException("Avaliação deve ser entre 1 e 5 estrelas.");
        
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Comentário da avaliação é obrigatório.");
        
        if (comment.Length > 1000)
            throw new DomainException("Comentário deve ter no máximo 1000 caracteres.");
        
        if (title?.Length > 100)
            throw new DomainException("Título deve ter no máximo 100 caracteres.");
        
        ProductId = productId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
        Title = title;
        UserName = userName;
        UserEmail = userEmail;
        IsVerifiedPurchase = isVerifiedPurchase;
        IsRecommended = isRecommended;
        Images = images;
        IsApproved = false;
        HelpfulCount = 0;
        NotHelpfulCount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(int? adminId = null)
    {
        IsApproved = true;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsHelpful(int userId)
    {
        if (_helpfulVotes.Any(v => v.UserId == userId))
            throw new DomainException("Usuário já votou nesta avaliação.");
        
        var vote = new ReviewHelpful(
            reviewId: Id,
            userId: userId,
            isHelpful: true
        );
        
        _helpfulVotes.Add(vote);
        HelpfulCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsNotHelpful(int userId)
    {
        if (_helpfulVotes.Any(v => v.UserId == userId))
            throw new DomainException("Usuário já votou nesta avaliação.");
        
        var vote = new ReviewHelpful(
            reviewId: Id,
            userId: userId,
            isHelpful: false
        );
        
        _helpfulVotes.Add(vote);
        NotHelpfulCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveHelpfulVote(int userId)
    {
        var vote = _helpfulVotes.FirstOrDefault(v => v.UserId == userId);
        if (vote == null)
            throw new DomainException("Voto não encontrado.");
        
        if (vote.IsHelpful)
            HelpfulCount--;
        else
            NotHelpfulCount--;
        
        _helpfulVotes.Remove(vote);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAdminResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new DomainException("Resposta do administrador é obrigatória.");
        
        if (response.Length > 500)
            throw new DomainException("Resposta deve ter no máximo 500 caracteres.");
        
        AdminResponse = response;
        AdminResponseAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Comentário é obrigatório.");
        
        if (comment.Length > 1000)
            throw new DomainException("Comentário deve ter no máximo 1000 caracteres.");
        
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Avaliação deve ser entre 1 e 5 estrelas.");
        
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }

    public double GetHelpfulnessPercentage()
    {
        var total = HelpfulCount + NotHelpfulCount;
        if (total == 0)
            return 0;
        
        return Math.Round((double)HelpfulCount / total * 100, 2);
    }

    public bool IsHelpful() => HelpfulCount > NotHelpfulCount;

    public override string ToString()
    {
        return $"{Rating}★ - {Title ?? Comment.Substring(0, Math.Min(50, Comment.Length))}...";
    }
}