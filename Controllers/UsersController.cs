using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TCS_Cliente.Data;
using TCS_Cliente.Models;

namespace TCS_Cliente.Controllers
{
    [Route("usuarios")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Dados inválidos" });
            }

            var existingUser = _context.Users.SingleOrDefault(u => u.Email == user.Email);

            if (existingUser != null)
            {
                return Conflict(new { mensagem = "Email já cadastrado" });
            }

            user.isAdmin = 0; // Define padrão para usuário comum
            _context.Users.Add(user);
            _context.SaveChanges();

            return StatusCode(201); // Retorna 201 Created sem corpo
        }

        [HttpGet("{email}")]
        [Authorize] // Certifique-se de que o usuário está autenticado
        public IActionResult GetUserByEmail(string email)
        {
            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o email do token corresponde ao email do usuário ou se o usuário é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);
            if (currentUser == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            // Permite que um admin acesse qualquer usuário, ou o próprio usuário acesse seus próprios dados
            if (currentUserEmail != email && currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Acesso negado. Apenas administradores podem acessar dados de outros usuários." });
            }

            // Busca o usuário pelo email
            var user = _context.Users
                .Where(u => u.Email == email)
                .Select(u => new { u.Email, u.Senha, u.Nome }) // Retorna apenas os campos necessários
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            return Ok(user); // Retorna os dados do usuário
        }


        [HttpPut("{email}")]
        [Authorize] // Certifique-se de que o usuário está autenticado
        public IActionResult UpdateUser(string email, [FromBody] UserUpdateRequest userUpdate)
        {
            if (userUpdate == null || (string.IsNullOrEmpty(userUpdate.Senha) && string.IsNullOrEmpty(userUpdate.Nome)))
            {
                return BadRequest(new { mensagem = "Pelo menos um campo (senha ou nome) deve ser fornecido" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Dados inválidos", erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o email do token corresponde ao email do usuário ou se o usuário é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);
            if (currentUser == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            // Permite que um admin atualize qualquer usuário, ou o próprio usuário atualize a si mesmo
            if (currentUserEmail != email && currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Acesso negado. Apenas administradores podem atualizar outros usuários." });
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            // Atualiza apenas os campos fornecidos
            if (!string.IsNullOrEmpty(userUpdate.Senha))
            {
                user.Senha = userUpdate.Senha;
            }

            if (!string.IsNullOrEmpty(userUpdate.Nome))
            {
                user.Nome = userUpdate.Nome;
            }

            _context.SaveChanges(); // Salva as mudanças no banco

            return Ok(new { user.Senha, user.Nome }); // Retorna os dados atualizados
        }


        [HttpDelete("{email}")]
        [Authorize] // Certifique-se de que o usuário está autenticado
        public IActionResult DeleteUser(string email)
        {
            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o email do token corresponde ao email do usuário ou se o usuário é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);
            if (currentUser == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            // Permite que um admin apague qualquer usuário, ou o próprio usuário apague a si mesmo
            if (currentUserEmail != email && currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Acesso negado. Apenas administradores podem excluir outros usuários." });
            }

            // Verifica se o email fornecido corresponde a um usuário existente
            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            _context.Users.Remove(user); // Remove o usuário do banco
            _context.SaveChanges(); // Salva a alteração no banco

            return Ok(); // Retorna um status 200 sem corpo (sem dados)
        }

        [HttpGet]
        [Authorize] // Certifique-se de que o usuário está autenticado
        public IActionResult GetAllUsers()
        {
            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o usuário autenticado é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);
            if (currentUser == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado" });
            }

            // Verifica se o usuário é um administrador
            if (currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Acesso negado. Apenas administradores podem acessar todos os usuários." });
            }

            // Retorna todos os usuários
            var users = _context.Users
                .Select(u => new { u.Email, u.Nome, u.Senha }) // Aqui você pode ajustar os dados retornados conforme necessário
                .ToList();

            return Ok(users); // Retorna todos os usuários
        }

    }
}

