using Domain.Exceptions;

namespace Domain.Entities.ValueObjects;

public class ProductSpecs
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string Color { get; private set; }
    public string Size { get; private set; }
    public string Material { get; private set; }
    public string Manufacturer { get; private set; }
    public DateTime? ManufactureDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    public ProductSpecs(
        string brand = null,
        string model = null,
        string color = null,
        string size = null,
        string material = null,
        string manufacturer = null,
        DateTime? manufactureDate = null,
        DateTime? expirationDate = null)
    {
        Brand = brand;
        Model = model;
        Color = color;
        Size = size;
        Material = material;
        Manufacturer = manufacturer;
        ManufactureDate = manufactureDate;
        ExpirationDate = expirationDate;
        
        ValidateDates();
    }

    public void Update(
        string brand = null,
        string model = null,
        string color = null,
        string size = null,
        string material = null,
        string manufacturer = null,
        DateTime? manufactureDate = null,
        DateTime? expirationDate = null)
    {
        Brand = brand;
        Model = model;
        Color = color;
        Size = size;
        Material = material;
        Manufacturer = manufacturer;
        ManufactureDate = manufactureDate;
        ExpirationDate = expirationDate;
        
        ValidateDates();
    }

    private void ValidateDates()
    {
        if (ManufactureDate.HasValue && ExpirationDate.HasValue)
        {
            if (ManufactureDate.Value >= ExpirationDate.Value)
                throw new DomainException("Data de fabricação deve ser anterior à data de validade.");
        }
        
        if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow)
            throw new DomainException("Data de validade não pode ser no passado.");
    }

    public bool IsExpired()
    {
        return ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
    }
}