using Domain.Exceptions;

namespace Domain.Validators;

public static class ProductValidator
{
    public static void Validate(string name, string description, decimal price, string barCode)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateBarCode(barCode);
    }

    public static void ValidateCategory(int categoryId)
    {
        if (categoryId <= 0)
            throw new DomainException("Categoria inválida.");
    }

    public static void ValidateId(int id)
    {
        if (id <= 0)
            throw new DomainException("Id inválido.");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
        
        if (name.Length < 3)
            throw new DomainException("O nome do produto precisa ter no mínimo 3 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Descrição é obrigatória.");

        if (description.Length < 3)
            throw new DomainException("A descrição precisa ter no mínimo 3 caracteres.");
    }

    private static void ValidatePrice(decimal price)
    {
        if (price == 0)
            throw new DomainException("O preço é obrigatório.");
        
        if (price <= 0)
            throw new DomainException("Preço deve ser maior que zero.");
    }

    private static void ValidateBarCode(string barCode)
    {
        if (string.IsNullOrWhiteSpace(barCode))
            throw new DomainException("EAN é obrigatório.");

        barCode = barCode.Trim();

        if (!barCode.All(char.IsDigit))
            throw new DomainException("EAN deve conter apenas números.");
        
        //if (barCode.Length != 8 && barCode.Length != 13)
        //    throw new DomainException("EAN deve ter 8 ou 13 dígitos.");
        
        //if (!IsValidEan(barCode))
        //    throw new DomainException("EAN inválido - dígito verificador não confere.");
    }

    private static bool IsValidEan(string barCode)
    {
        if (string.IsNullOrWhiteSpace(barCode))
            return false;

        barCode = new string(barCode.Where(char.IsDigit).ToArray());

        if (barCode.Length != 8 && barCode.Length != 13)
            return false;

        int sum = 0;
        // Correção: Para EAN-13 deve ser 1, não 2
        int multiplier = barCode.Length == 13 ? 3 : 3;

        for (int i = barCode.Length - 2; i >= 0; i--)
        {
            int digit = int.Parse(barCode[i].ToString());
            sum += digit * multiplier;
            
            // Alterna o multiplicador entre 1 e 3
            multiplier = multiplier == 1 ? 3 : 1;
        }
        
        int checkDigit = (10 - (sum % 10)) % 10;
        int lastDigit = int.Parse(barCode[barCode.Length - 1].ToString());
        
        return checkDigit == lastDigit;
    }
}