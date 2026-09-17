using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _secretKey;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _secretKey = configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("JWT secret key is not configured.");
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null; // Credenciais inválidas
            }

            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<string> RegisterUserAsync(string username, string email, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                return null; 
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = "User" // Role padrão, você pode modificar conforme a lógica
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            var token = GenerateJwtToken(createdUser);

            return token; // Retorna o token JWT do novo usuário
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // IMPORTANTE: Recalcula o hash da senha fornecida
            var computedHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            return passwordHash == computedHash;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "unique-app-id-12345",
                audience: "MyAppUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
