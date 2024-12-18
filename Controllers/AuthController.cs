using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TCS_Cliente.Models;
using TCS_Cliente.Services;

namespace TCS_Cliente.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var token = _userService.Authenticate(login.Email, login.Senha);

            if (token == null)
            {
                return Unauthorized(new { mensagem = "Email e/ou senha inválidos" });
            }

            return Ok(new { Token = token }); // Retorna o token gerado
        }

        [HttpPost("logout")]
        [Route("logout")]
        public IActionResult Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(); // Token ausente ou malformado
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(token))
            {
                return Unauthorized(); // Token inválido
            }

            return Ok();
        }
    }
}
