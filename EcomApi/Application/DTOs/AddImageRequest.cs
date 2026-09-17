using Microsoft.AspNetCore.Http;
namespace Application.DTOs;

public class AddImageRequest
{
    public IFormFile Image { get; set; } = null!;
    public bool IsMain { get; set; } = false;
    public string? Description { get; set; }
    public int Order { get; set; } = 0;
}