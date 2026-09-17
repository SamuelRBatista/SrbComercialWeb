using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DapperContext _context;

    public CategoryRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        const string query = "SELECT id, name FROM categories ORDER BY name";
        
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Category>(query);
    }

    // Corrigir para retornar Task<Category?> em vez de Task<Category>
    public async Task<Category?> GetByIdAsync(int id)
    {
        const string query = "SELECT id, name FROM categories WHERE id = @Id";
        
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Category>(query, new { Id = id });
    }

    public async Task<Category> AddAsync(Category category)
    {
        const string query = @"
            INSERT INTO categories (name) 
            VALUES (@Name) 
            RETURNING id";
        
        using var connection = _context.CreateConnection();
        var id = await connection.QuerySingleAsync<int>(query, new { category.Name });
        
        category.Id = id;
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        const string query = "UPDATE categories SET name = @Name WHERE id = @Id";
        
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { category.Name, category.Id });
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM categories WHERE id = @Id";
        
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }
}