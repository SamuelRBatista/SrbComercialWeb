using Application.DTOs;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly SupplierService _supplierService;
    private readonly IValidator<SupplierCreateRequest> _createValidator;
    private readonly IValidator<SupplierUpdateRequest> _updateValidator;

    public SupplierController(
        SupplierService supplierService,
        IValidator<SupplierCreateRequest> createValidator,
        IValidator<SupplierUpdateRequest> updateValidator)
    {
        _supplierService = supplierService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierResponseDto>>> GetSuppliers()
    {
        var suppliers = await _supplierService.GetAllAsync();

        var response = suppliers.Select(s => new SupplierResponseDto
        {
            Id = s.Id,
            Cnpj = s.Cnpj,
            Name = s.Name,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            Neighborhood = s.Neighborhood,
            ZipCode = s.ZipCode,
            StateId = s.StateId,
            StateName = s.State?.Name,
            StateUf = s.State?.Uf,
            CityId = s.CityId,
            CityName = s.City?.Name
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierResponseDto>> GetSupplierById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);

        if (supplier == null)
        {
            return NotFound(new
            {
                Message = "Fornecedor não encontrado."
            });
        }

        var response = new SupplierResponseDto
        {
            Id = supplier.Id,
            Cnpj = supplier.Cnpj,
            Name = supplier.Name,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address,
            Neighborhood = supplier.Neighborhood,
            ZipCode = supplier.ZipCode,
            StateId = supplier.StateId,
            StateName = supplier.State?.Name,
            StateUf = supplier.State?.Uf,
            CityId = supplier.CityId,
            CityName = supplier.City?.Name
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponseDto>> AddSupplierAsync(
        [FromBody] SupplierCreateRequest request)
    {
        try
        {
            var validationResult =
                await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var supplier = new Supplier(
                request.Cnpj,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.Neighborhood,
                request.ZipCode,
                request.StateId,
                request.CityId
            );

            await _supplierService.AddAsync(supplier);

            var response = new SupplierResponseDto
            {
            Id = supplier.Id,
            Cnpj = supplier.Cnpj,
            Name = supplier.Name,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address,
            Neighborhood = supplier.Neighborhood,
            ZipCode = supplier.ZipCode,
            StateId = supplier.StateId,
            CityId = supplier.CityId
           };

         return CreatedAtAction(
            nameof(GetSupplierById),
            new { id = supplier.Id },
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
    public async Task<IActionResult> UpdateSupplierAsync(
        int id,
        [FromBody] SupplierUpdateRequest request)
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

            var existingSupplier =
                await _supplierService.GetByIdAsync(id);

            if (existingSupplier == null)
            {
                return NotFound(new
                {
                    Error = "Fornecedor não encontrado."
                });
            }

            existingSupplier.Update(
                request.Cnpj,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.Neighborhood,
                request.ZipCode,
                request.StateId,
                request.CityId
            );

            await _supplierService.UpdateAsync(existingSupplier);

            return Ok(new
            {
                Message = "Fornecedor atualizado com sucesso."
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
    public async Task<IActionResult> DeleteSupplierAsync(int id)
    {
        try
        {
            var existingSupplier =
                await _supplierService.GetByIdAsync(id);

            if (existingSupplier == null)
            {
                return NotFound(new
                {
                    Error = "Fornecedor não encontrado."
                });
            }

            await _supplierService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Fornecedor deletado com sucesso."
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