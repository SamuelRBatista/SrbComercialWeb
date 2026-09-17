using System.Security.Cryptography.X509Certificates;
using Domain.Exceptions;

namespace Domain.Validators;

public static class SupplierValidator
{
    
    public static void Validator(
        string name,
        string cnpj,
        string email,
        string phoneNumber,
        string address,
        string neighborhood,   
        string zipCode,
        int stateId,
        int cityId)
    {

        ValidateName(name);
        //ValidateCnpj(cnpj);
        ValidateEmail(email);
        ValidatePhoneNumber(phoneNumber);
        ValidateAddress(address);
        ValidateNeighborhood(neighborhood);
        ValidateZipCode(zipCode);   
        ValidateCity(cityId);
        ValidateState(stateId);
    }

    public static void ValidateCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório.");

        if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            throw new DomainException("CNPJ inválido.");
    }

    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
        if (name.Length < 3)
            throw new DomainException("Nome deve possuir no mínimo 3 caracteres.");
    }  
        
    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");
        if (!email.Contains("@"))
            throw new DomainException("E-mail inválido.");
    } 

    public static void ValidatePhoneNumber(string phoneNumber)
    {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Telefone é obrigatório.");
    } 

    public static void ValidateAddress(string address)
    {
            if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Endereço é obrigatório.");
    }
    
    public static void ValidateNeighborhood(string neighborhood)
    {
        if (string.IsNullOrWhiteSpace(neighborhood))
            throw new DomainException("Bairro é obrigatório.");
    }
       
    public static void ValidateZipCode(string zipCode)
    {
           if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("CEP é obrigatório.");
    } 
   
    public static void ValidateCity(int cityId)
    {
          if (cityId <= 0)
            throw new DomainException("Cidade inválida.");
    }  

      public static void ValidateState(int stateId)
    {
        if (stateId <= 0)
         throw new DomainException("Id inválido.");
    }  


}
