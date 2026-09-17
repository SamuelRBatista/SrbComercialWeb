using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly DapperContext _context;

    public SupplierRepository(DapperContext context)
    {
        _context = context;
    }

    private const string BaseQuery = @"
        SELECT
            s.id,
            s.cnpj,
            s.name,
            s.email,
            s.phone_number,
            s.address,
            s.neighborhood,
            s.zip_code AS ZipCode,
            s.state_id AS StateId,
            s.city_id AS CityId,

            st.id AS StateKey,
            st.name AS StateName,
            st.uf AS StateUf,

            c.id AS CityKey,
            c.name AS CityName,
            c.state_id AS CityStateId

        FROM suppliers s
        INNER JOIN states st ON s.state_id = st.id
        INNER JOIN cities c ON s.city_id = c.id
    ";

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        var query = $"{BaseQuery} ORDER BY s.name";

        using var connection = _context.CreateConnection();

        var dict = new Dictionary<int, Supplier>();

        await connection.QueryAsync<Supplier, State, City, Supplier>(
            query,
            (supplier, state, city) =>
            {
                if (!dict.TryGetValue(supplier.Id, out var current))
                {
                    current = supplier;

                    current.SetState(state);
                    current.SetCity(city);

                    dict.Add(current.Id, current);
                }

                return current;
            },
            splitOn: "StateKey,CityKey"
        );

        return dict.Values;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        var query = $"{BaseQuery} WHERE s.id = @Id";

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<Supplier, State, City, Supplier>(
            query,
            (supplier, state, city) =>
            {
                supplier.SetState(state);
                supplier.SetCity(city);
                return supplier;
            },
            new { Id = id },
            splitOn: "StateKey,CityKey"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Supplier>> GetByCityIdAsync(int cityId)
    {
        var query = $"{BaseQuery} WHERE s.city_id = @CityId ORDER BY s.name";

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Supplier, State, City, Supplier>(
            query,
            (supplier, state, city) =>
            {
                supplier.SetState(state);
                supplier.SetCity(city);
                return supplier;
            },
            new { CityId = cityId },
            splitOn: "StateKey,CityKey"
        );
    }

    public async Task<IEnumerable<Supplier>> GetByStateIdAsync(int stateId)
    {
        var query = $"{BaseQuery} WHERE s.state_id = @StateId ORDER BY s.name";

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Supplier, State, City, Supplier>(
            query,
            (supplier, state, city) =>
            {
                supplier.SetState(state);
                supplier.SetCity(city);
                return supplier;
            },
            new { StateId = stateId },
            splitOn: "StateKey,CityKey"
        );
    }

    public async Task<Supplier> AddAsync(Supplier supplier)
    {
        const string query = @"
            INSERT INTO suppliers
            (
                cnpj,
                name,
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
                @Cnpj,
                @Name,
                @Email,
                @PhoneNumber,
                @Address,
                @Neighborhood,
                @ZipCode,
                @StateId,
                @CityId
            )
            RETURNING id;
        ";

        using var connection = _context.CreateConnection();

        var id = await connection.QuerySingleAsync<int>(query, new
        {
            supplier.Cnpj,
            supplier.Name,
            supplier.Email,
            supplier.PhoneNumber,
            supplier.Address,
            supplier.Neighborhood,
            supplier.ZipCode,
            supplier.StateId,
            supplier.CityId
        });

        supplier.SetId(id);
        return supplier;
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        const string query = @"
            UPDATE suppliers
            SET
                cnpj = @Cnpj,
                name = @Name,
                email = @Email,
                phone_number = @PhoneNumber,
                address = @Address,
                neighborhood = @Neighborhood,
                zip_code = @ZipCode,
                state_id = @StateId,
                city_id = @CityId
            WHERE id = @Id";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, new
        {
            supplier.Id,
            supplier.Cnpj,
            supplier.Name,
            supplier.Email,
            supplier.PhoneNumber,
            supplier.Address,
            supplier.Neighborhood,
            supplier.ZipCode,
            supplier.StateId,
            supplier.CityId
        });
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM suppliers WHERE id = @Id";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, new { Id = id });
    }
}