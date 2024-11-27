using Microsoft.AspNetCore.Mvc;
using TCS_Cliente.Models;
using TCS_Cliente.Services;

namespace TCS_Cliente.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var token = _userService.Authenticate(login.Email, login.Senha);

            if (token == null)
            {
                return Unauthorized(); // Retorna 401 se não encontrar o usuário
            }

            return Ok(new { Token = token }); // Retorna o token gerado
        }
    }
}
