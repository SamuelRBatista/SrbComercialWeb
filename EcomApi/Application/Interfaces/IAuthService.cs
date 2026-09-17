namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
          
        Task<string> RegisterUserAsync(string username, string email, string password);
    }
}
