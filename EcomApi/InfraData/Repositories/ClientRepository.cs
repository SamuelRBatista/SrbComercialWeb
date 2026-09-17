using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class ClientRepository : IClientReposiory
{
    private readonly DapperContext _context;

    public ClientRepository(DapperContext context)
    {
        _context = context;
    }

 private const string BaseQuery = @"
    SELECT
        c.id,
        c.name,
        c.cpf,
        c.email,
        c.phone_number AS PhoneNumber,
        c.address,
        c.neighborhood AS Neighborhood,
        c.zip_code AS ZipCode,
        c.state_id AS StateId,
        c.city_id AS CityId,

        s.id AS StateSplit,
        s.id,
        s.name,
        s.uf,

        ci.id AS CitySplit,
        ci.id,
        ci.name,
        ci.state_id AS StateId

    FROM clients c
    INNER JOIN states s ON c.state_id = s.id
    INNER JOIN cities ci ON c.city_id = ci.id";

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var query = $"{BaseQuery} ORDER BY c.name";

        using var connection = _context.CreateConnection();

        var clientDictionary = new Dictionary<int, Client>();

        await connection.QueryAsync<Client, State, City, Client>(
            query,
            (client, state, city) =>
            {
                if (!clientDictionary.TryGetValue(client.Id, out var currentClient))
                {
                    currentClient = client;

                    currentClient.SetState(state);
                    currentClient.SetCity(city);

                    clientDictionary.Add(currentClient.Id, currentClient);
                }

                return currentClient;
            },
            splitOn: "StateSplit,CitySplit"
        );

        return clientDictionary.Values;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        var query = $"{BaseQuery} WHERE c.id = @Id";

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<Client, State, City, Client>(
            query,
            (client, state, city) =>
            {
                client.SetState(state);
                client.SetCity(city);

                return client;
            },
            new { Id = id },
            splitOn: "StateSplit,CitySplit"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Client>> GetByCityIdAsync(int cityId)
    {
        var query = $"{BaseQuery} WHERE c.city_id = @CityId ORDER BY c.name";

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Client, State, City, Client>(
            query,
            (client, state, city) =>
            {
                client.SetState(state);
                client.SetCity(city);

                return client;
            },
            new { CityId = cityId },
            splitOn: "StateSplit,CitySplit"
        );
    }

    public async Task<IEnumerable<Client>> GetByStateIdAsync(int stateId)
    {
        var query = $"{BaseQuery} WHERE c.state_id = @StateId ORDER BY c.name";

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Client, State, City, Client>(
            query,
            (client, state, city) =>
            {
                client.SetState(state);
                client.SetCity(city);

                return client;
            },
            new { StateId = stateId },
            splitOn: "StateSplit,CitySplit"
        );
    }

    public async Task<Client> AddAsync(Client client)
    {
        const string query = @"
            INSERT INTO clients
            (
                name,
                cpf,
                email,
                phone_number,
                address,
                neighborhood,
                zip_code,
                state_id,
                city_id
            )
            VALUES
            (
                @Name,
                @Cpf,
                @Email,
                @PhoneNumber,
                @Address,
                @Neighborhood,
                @ZipCode,
                @StateId,
                @CityId
            )
            RETURNING id;";

        using var connection = _context.CreateConnection();

        var id = await connection.QuerySingleAsync<int>(
            query,
            new
            {
                client.Name,
                client.Cpf,
                client.Email,
                client.PhoneNumber,
                client.Address,
                client.Neighborhood,
                client.ZipCode,
                client.StateId,
                client.CityId
            });

        client.SetId(id);

        return client;
    }

    public async Task UpdateAsync(Client client)
    {
        const string query = @"
            UPDATE clients
            SET
                name = @Name,
                cpf = @Cpf,
                email = @Email,
                phone_number = @PhoneNumber,
                address = @Address,
                neighborhood = @Neighborhood,
                zip_code = @ZipCode,
                state_id = @StateId,
                city_id = @CityId
            WHERE id = @Id";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            query,
            new
            {
                client.Id,
                client.Name,
                client.Cpf,
                client.Email,
                client.PhoneNumber,
                client.Address,
                client.Neighborhood,
                client.ZipCode,
                client.StateId,
                client.CityId
            });
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM clients WHERE id = @Id";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, new { Id = id });
    }
}