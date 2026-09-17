namespace Application.DTOs;

public class ClientCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int StateId { get; set; }
    public int CityId { get; set; }
}