namespace Application.DTOs;

public class SupplierResponseDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int StateId { get; set; }
    public string? StateName { get; set; }
    public string? StateUf { get; set; }
    public int CityId { get; set; }
    public string? CityName { get; set; }
}