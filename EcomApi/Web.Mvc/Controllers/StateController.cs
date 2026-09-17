using Application.Services;
using Domain.Entities;  // Referência para o Product
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class StateController : ControllerBase
{
    private readonly StateService _stateService;

    public StateController(StateService stateService)
    {
        _stateService = stateService;
    }
      
    [HttpGet]
  
    public async Task<ActionResult<IEnumerable<State>>> GetStates()
    {
        var states = await _stateService.GetAllAsync();
        return Ok(states);
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
