using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class StateRepository : IStateReposiory
{
    private readonly DapperContext _context;

    public StateRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<State>> GetAllAsync()
    {
        const string query = "SELECT id, name, uf FROM states ORDER BY name";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<State>(query);
    }

    public async Task<State?> GetByIdAsync(int id)
    {
        const string query = "SELECT id, name, uf FROM states WHERE id = @Id";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<State>(query, new { Id = id });
    }

    public async Task<State?> GetByUfAsync(string uf)
    {
        // Implementação do método GetByUfAsync
        const string query = "SELECT id, name, uf FROM states WHERE uf = @Uf";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<State>(query, new { Uf = uf.ToUpper() });
    }
}