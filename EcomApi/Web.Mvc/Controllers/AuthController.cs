using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Mvc.Models;  // Modelo para LoginModel, se necessário

namespace Web.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Endpoint para autenticação
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Credenciais inválidas");
            }

            // Autentica o usuário e gera o token JWT
            var token = await _authService.AuthenticateAsync(loginModel.Username, loginModel.Password);

            if (token == null)
            {
                return Unauthorized("Credenciais inválidas");
            }

            // Retorna o token JWT
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel == null || string.IsNullOrEmpty(registerModel.Username) || string.IsNullOrEmpty(registerModel.Password))
            {
                return BadRequest("Informações de registro inválidas");
            }

            var token = await _authService.RegisterUserAsync(registerModel.Username, registerModel.Email, registerModel.Password);

            if (token == null)
            {
                return Conflict("Usuário já existe");
            }

            return Ok(new { Token = token });
        }
    }
}
