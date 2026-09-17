using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id inválido.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Preço deve ser maior que zero.");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU é obrigatório.");

        RuleFor(x => x.BarCode)
            .Length(13)
            .WithMessage("Código de barras deve possuir 13 dígitos.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Categoria inválida.");
    }
}