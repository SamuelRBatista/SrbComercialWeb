using Domain.Exceptions;
using Domain.Validators;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Client
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Cpf { get; private set; }

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

    protected Client(){}

    public Client(
        string name,
        string cpf,
        string email,
        string phoneNumber,
        string address,
        string neighborhood,
        string zipCode,
        int stateId,
        int cityId)
    {

        ClientValidator.Validator(
            name,
            cpf,
            email,
            phoneNumber,
            address,
            neighborhood,
            zipCode,
            stateId,
            cityId);
        
        ClientValidator.ValidateCity(cityId);
        ClientValidator.ValidateState(stateId);
 

        Name = name;
        Cpf = cpf;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        StateId = stateId;
        CityId = cityId;
    }

    public void Update(
        string name,
        string cpf,
        string email,
        string phoneNumber,
        string address,
        string neighborhood,
        string zipCode,
        int stateId,
        int cityId)
    {
        
         ClientValidator.Validator(
            name,
            cpf,
            email,
            phoneNumber,
            address,
            neighborhood,
            zipCode,
            stateId,
            cityId);
        
        ClientValidator.ValidateCity(cityId);
        ClientValidator.ValidateState(stateId);

        Name = name;
        Cpf = cpf;
        Email = email;
        PhoneNumber = phoneNumber;
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