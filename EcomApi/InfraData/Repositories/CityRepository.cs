using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class CityRepository : ICityRepository
{
    private readonly DapperContext _context;

    public CityRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<City>> GetAllAsync()
    {
        const string query = @"
            SELECT 
                c.id,
                c.name,
                c.state_id,
                s.id as StateId,
                s.name as StateName,
                s.uf as StateUf
            FROM cities c
            INNER JOIN states s ON c.state_id = s.id
            ORDER BY s.name, c.name";

        using var connection = _context.CreateConnection();

        var results = await connection.QueryAsync<dynamic>(query);

        var cities = new List<City>();

        foreach (var row in results)
        {
            // Tratar valores nulos
            var city = new City
            {
                Id = row.id != null ? (int)row.id : 0,
                Name = row.name != null ? (string)row.name : string.Empty,
                StateId = row.state_id != null ? (int)row.state_id : 0,
                State = new State
                {
                    Id = row.StateId != null ? (int)row.StateId : 0,
                    Name = row.StateName != null ? (string)row.StateName : string.Empty,
                    Uf = row.StateUf != null ? (string)row.StateUf : string.Empty
                }
            };
            cities.Add(city);
        }

        return cities;
    }

    public async Task<City?> GetByIdAsync(int id)
    {
        const string query = @"
            SELECT 
                c.id,
                c.name,
                c.state_id,
                s.id as StateId,
                s.name as StateName,
                s.uf as StateUf
            FROM cities c
            INNER JOIN states s ON c.state_id = s.id
            WHERE c.id = @Id";

        using var connection = _context.CreateConnection();

        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Id = id });

        if (row == null)
            return null;

        var city = new City
        {
            Id = row.id != null ? (int)row.id : 0,
            Name = row.name != null ? (string)row.name : string.Empty,
            StateId = row.state_id != null ? (int)row.state_id : 0,
            State = new State
            {
                Id = row.StateId != null ? (int)row.StateId : 0,
                Name = row.StateName != null ? (string)row.StateName : string.Empty,
                Uf = row.StateUf != null ? (string)row.StateUf : string.Empty
            }
        };

        return city;
    }

    public async Task<IEnumerable<City>> GetByStateIdAsync(int stateId)
    {
        const string query = @"
            SELECT 
                c.id,
                c.name,
                c.state_id,
                s.id as StateId,
                s.name as StateName,
                s.uf as StateUf
            FROM cities c
            INNER JOIN states s ON c.state_id = s.id
            WHERE c.state_id = @StateId
            ORDER BY c.name";

        using var connection = _context.CreateConnection();

        var results = await connection.QueryAsync<dynamic>(query, new { StateId = stateId });

        var cities = new List<City>();

        foreach (var row in results)
        {
            var city = new City
            {
                Id = row.id != null ? (int)row.id : 0,
                Name = row.name != null ? (string)row.name : string.Empty,
                StateId = row.state_id != null ? (int)row.state_id : 0,
                State = new State
                {
                    Id = row.StateId != null ? (int)row.StateId : 0,
                    Name = row.StateName != null ? (string)row.StateName : string.Empty,
                    Uf = row.StateUf != null ? (string)row.StateUf : string.Empty
                }
            };
            cities.Add(city);
        }

        return cities;
    }

    public async Task<City> AddAsync(City city)
    {
        const string query = @"
            INSERT INTO cities (name, state_id) 
            VALUES (@Name, @StateId)
            RETURNING id;";

        using var connection = _context.CreateConnection();
        var id = await connection.QuerySingleAsync<int>(query, new
        {
            city.Name,
            city.StateId
        });

        city.Id = id;
        return city;
    }

    public async Task UpdateAsync(City city)
    {
        const string query = @"
            UPDATE cities 
            SET name = @Name, state_id = @StateId 
            WHERE id = @Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new
        {
            city.Name,
            city.StateId,
            city.Id
        });
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM cities WHERE id = @Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }
}