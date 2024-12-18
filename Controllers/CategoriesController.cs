using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TCS_Cliente.Data;
using TCS_Cliente.Models;

namespace TCS_Cliente.Controllers
{
    [Route("categorias")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize] // Garante que o usuário está autenticado
        public IActionResult Register([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Dados inválidos" });
            }

            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o usuário autenticado é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            if (currentUser == null || currentUser.isAdmin != 1) // Verifica se é admin
            {
                return Unauthorized(new { mensagem = "Você não tem permissão suficiente para performar esta ação" });

            }

            // Adiciona a nova categoria ao banco
            _context.Categories.Add(category);
            _context.SaveChanges();

            return StatusCode(201); // Retorna 201 Created
        }

        [HttpPut("{id}")]
        [Authorize] // Garante que o usuário está autenticado
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Dados inválidos" });
            }

            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o usuário autenticado é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            if (currentUser == null || currentUser.isAdmin != 1) // Verifica se é admin
            {
                return Unauthorized(new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            // Busca a categoria no banco
            var existingCategory = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            // Atualiza a categoria
            existingCategory.Nome = category.Nome; // Ajuste conforme os campos necessários

            _context.SaveChanges();

            return Ok(existingCategory); // Retorna a categoria atualizada
        }

        [HttpDelete("{id}")]
        [Authorize] // Garante que o usuário está autenticado
        public IActionResult DeleteCategory(int id)
        {
            // Obtém o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Verifica se o usuário autenticado é um administrador
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            if (currentUser == null || currentUser.isAdmin != 1) // Verifica se é admin
            {
                return Unauthorized(new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            // Busca a categoria no banco
            var category = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            // Remover a categoria do banco
            _context.Categories.Remove(category);
            _context.SaveChanges();

            // Retorna uma resposta de sucesso com a categoria removida
            return Ok(new { mensagem = "Categoria excluída com sucesso" });
        }
    }
}
