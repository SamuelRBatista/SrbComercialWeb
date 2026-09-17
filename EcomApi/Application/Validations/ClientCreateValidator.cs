using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class ClientCreateValidator
    : AbstractValidator<ClientCreateRequest>
{
    public ClientCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .Must(cpf =>
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return false;

                cpf = new string(cpf.Where(char.IsDigit).ToArray());

                return cpf.Length == 11;
            })
            .WithMessage("CPF deve possuir 11 dígitos.");

        RuleFor(x => x.Email)
            .NotEmpty()
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