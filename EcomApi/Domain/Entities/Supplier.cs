using Domain.Exceptions;
using Domain.Validators;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string Cnpj { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public string Address { get; private set; }

    public string Neighborhood { get; private set; }

    public string ZipCode { get; private set; }

    public int StateId { get; private set; }

    [JsonIgnore]
    public State? State { get; private set; }

    public int CityId { get; private set; }

    [JsonIgnore]
    public City? City { get; private set; }

    protected Supplier()
    {
    }

    public Supplier(
        string cnpj,
        string name,
        string email,
        string phonenumber,
        string address,
        string neighborhood,
        string zipCode,
        int stateId,
        int cityId)
    {
         SupplierValidator.Validator(
            name,
            cnpj,
            email,
            phonenumber,
            address,
            neighborhood,
            zipCode,
            stateId,
            cityId);
        
        ClientValidator.ValidateCity(cityId);
        ClientValidator.ValidateState(stateId);

        Cnpj = cnpj;
        Name = name;
        Email = email;
        PhoneNumber = phonenumber;
        Address = address;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        StateId = stateId;
        CityId = cityId;
    }

    public void Update(
        string cnpj,
        string name,
        string email,
        string phonenumber,
        string address,
        string neighborhood,
        string zipCode,
        int stateId,
        int cityId)
    {
         SupplierValidator.Validator(
            name,
            cnpj,
            email,
            phonenumber,
            address,
            neighborhood,
            zipCode,
            stateId,
            cityId);
        
        ClientValidator.ValidateCity(cityId);
        ClientValidator.ValidateState(stateId);

        Cnpj = cnpj;
        Name = name;
        Email = email;
        PhoneNumber = phonenumber;
        Address = address;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        StateId = stateId;
        CityId = cityId;
    }

    public void SetId(int id)
    {
        if (id <= 0)
            throw new DomainException("Id inválido.");

        Id = id;
    }

    public void SetState(State state)
    {
        State = state;
    }

    public void SetCity(City city)
    {
        City = city;
    }
    
}