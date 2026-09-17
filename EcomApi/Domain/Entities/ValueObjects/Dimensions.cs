using Domain.Exceptions;
namespace Domain.Entities.ValueObjects;

public class Dimensions
{
    public decimal? Weight { get; private set; }
    public decimal? Height { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? Depth { get; private set; }
    public string FormattedDimensions => GetFormattedDimensions();

    public Dimensions(
        decimal? weight = null,
        decimal? height = null,
        decimal? width = null,
        decimal? depth = null)
    {
        Weight = weight;
        Height = height;
        Width = width;
        Depth = depth;
    }

    public void Update(
        decimal? weight = null,
        decimal? height = null,
        decimal? width = null,
        decimal? depth = null)
    {
        if (weight.HasValue && weight.Value <= 0)
            throw new DomainException("Peso deve ser maior que zero.");
        
        if (height.HasValue && height.Value <= 0)
            throw new DomainException("Altura deve ser maior que zero.");
        
        if (width.HasValue && width.Value <= 0)
            throw new DomainException("Largura deve ser maior que zero.");
        
        if (depth.HasValue && depth.Value <= 0)
            throw new DomainException("Profundidade deve ser maior que zero.");
        
        Weight = weight;
        Height = height;
        Width = width;
        Depth = depth;
    }

    private string GetFormattedDimensions()
    {
        if (!Height.HasValue && !Width.HasValue && !Depth.HasValue)
            return null;
        
        return $"{Height ?? 0}x{Width ?? 0}x{Depth ?? 0} cm";
    }

    public decimal? GetVolume()
    {
        if (!Height.HasValue || !Width.HasValue || !Depth.HasValue)
            return null;
        
        return Height.Value * Width.Value * Depth.Value;
    }
}