using Application.Services;
using Domain.Entities;  // Referência para o Product
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly CityService _cityService;

    public CityController(CityService cityService)
    {
        _cityService = cityService;
    }

  
    [HttpGet]
  
    public async Task<ActionResult<IEnumerable<City>>> GetCities()
    {
        var city = await _cityService.GetAllAsync();
        return Ok(city);
    }
    
  
//     [HttpPost]
//     [Authorize] 
//     public async Task<ActionResult<Product>> AddProductAsync(Client client)
//     {
//         if(client == null){
//             return BadRequest("Produto inválido. ");
//         }
//         await _clientService.AddAsync(client);
       
//         return Ok("Client cadastrado com sucesso.");
//     }
 }
