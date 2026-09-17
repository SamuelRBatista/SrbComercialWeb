using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class ClientUpdateValidator
    : AbstractValidator<ClientUpdateRequest>
{
    public ClientUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .Length(11);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.Neighborhood)
            .NotEmpty();

        RuleFor(x => x.ZipCode)
            .NotEmpty();

        RuleFor(x => x.StateId)
            .GreaterThan(0);

        RuleFor(x => x.CityId)
            .GreaterThan(0);
    }
}