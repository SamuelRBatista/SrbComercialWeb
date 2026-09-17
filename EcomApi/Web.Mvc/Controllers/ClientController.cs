using Application.DTOs;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly ClientService _clientService;
    private readonly IValidator<ClientCreateRequest> _createValidator;
    private readonly IValidator<ClientUpdateRequest> _updateValidator;

    public ClientController(
        ClientService clientService,
        IValidator<ClientCreateRequest> createValidator,
        IValidator<ClientUpdateRequest> updateValidator)
    {
        _clientService = clientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetClients()
    {
        var clients = await _clientService.GetAllAsync();

        var response = clients.Select(c => new ClientResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Cpf = c.Cpf,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            Neighborhood = c.Neighborhood,
            ZipCode = c.ZipCode,
            StateId = c.StateId,
            StateName = c.State?.Name,
            StateUf = c.State?.Uf,
            CityId = c.CityId,
            CityName = c.City?.Name
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponseDto>> GetClientById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);

        if (client == null)
        {
            return NotFound(new
            {
                Message = "Cliente não encontrado."
            });
        }

        var response = new ClientResponseDto
        {
            Id = client.Id,
            Name = client.Name,
            Cpf = client.Cpf,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            Address = client.Address,
            Neighborhood = client.Neighborhood,
            ZipCode = client.ZipCode,
            StateId = client.StateId,
            StateName = client.State?.Name,
            StateUf = client.State?.Uf,
            CityId = client.CityId,
            CityName = client.City?.Name
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponseDto>> AddClientAsync(
        [FromBody] ClientCreateRequest request)
    {
        Console.WriteLine($"CPF recebido: '{request.Cpf}'");

        try
        {
            var validationResult =
                await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var client = new Client(
                request.Name,
                request.Cpf,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.Neighborhood,
                request.ZipCode,
                request.StateId,
                request.CityId
            );

            var created = await _clientService.AddAsync(client);

            var response = new ClientResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Cpf = created.Cpf,
                Email = created.Email,
                PhoneNumber = created.PhoneNumber,
                Address = created.Address,
                Neighborhood = created.Neighborhood,
                ZipCode = created.ZipCode,
                StateId = created.StateId,
                CityId = created.CityId
            };

            return CreatedAtAction(
                nameof(GetClientById),
                new { id = created.Id },
                response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClientAsync(
        int id,
        [FromBody] ClientUpdateRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(new
                {
                    Error = "O ID informado não corresponde ao ID da URL."
                });
            }

            var validationResult =
                await _updateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var existingClient =
                await _clientService.GetByIdAsync(id);

            if (existingClient == null)
            {
                return NotFound(new
                {
                    Error = "Cliente não encontrado."
                });
            }

            existingClient.Update(
                request.Name,
                request.Cpf,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.Neighborhood,
                request.ZipCode,
                request.StateId,
                request.CityId
            );

            await _clientService.UpdateAsync(existingClient);

            return Ok(new
            {
                Message = "Cliente atualizado com sucesso."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClientAsync(int id)
    {
        try
        {
            var existingClient =
                await _clientService.GetByIdAsync(id);

            if (existingClient == null)
            {
                return NotFound(new
                {
                    Error = "Cliente não encontrado."
                });
            }

            await _clientService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Cliente deletado com sucesso."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    private ActionResult ValidationError(
        ValidationResult validationResult)
    {
        return BadRequest(new
        {
            Errors = validationResult.Errors.Select(x => new
            {
                Campo = x.PropertyName,
                Mensagem = x.ErrorMessage
            })
        });
    }
}