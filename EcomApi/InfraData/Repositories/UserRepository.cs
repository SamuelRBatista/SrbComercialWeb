using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using InfraData.Context;

namespace InfraData.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        // CORRIGIDO: Mapear password_hash para PasswordHash
        const string query = @"
            SELECT 
                id, 
                username, 
                password_hash as PasswordHash, 
                email, 
                role 
            FROM users 
            WHERE username = @Username";

        using var connection = _context.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });

        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        // CORRIGIDO: Mapear password_hash para PasswordHash
        const string query = @"
            SELECT 
                id, 
                username, 
                password_hash as PasswordHash, 
                email, 
                role 
            FROM users 
            WHERE id = @Id";

        using var connection = _context.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });

        return user;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        const string query = @"
            INSERT INTO users (username, password_hash, email, role) 
            VALUES (@Username, @PasswordHash, @Email, @Role)
            RETURNING id;";

        using var connection = _context.CreateConnection();

        var id = await connection.QuerySingleAsync<int>(query, new
        {
            user.Username,
            user.PasswordHash,
            user.Email,
            user.Role
        });

        user.Id = id;
        return user;
    }
}