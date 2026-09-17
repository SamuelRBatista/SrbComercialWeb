using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class SupplierUpdateValidator
    : AbstractValidator<SupplierUpdateRequest>
{
    public SupplierUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id inválido.");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("E-mail inválido.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Telefone é obrigatório.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Endereço é obrigatório.");

        RuleFor(x => x.Neighborhood)
            .NotEmpty()
            .WithMessage("Bairro é obrigatório.");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("CEP é obrigatório.");

        RuleFor(x => x.StateId)
            .GreaterThan(0)
            .WithMessage("Estado inválido.");

        RuleFor(x => x.CityId)
            .GreaterThan(0)
            .WithMessage("Cidade inválida.");
    }
}