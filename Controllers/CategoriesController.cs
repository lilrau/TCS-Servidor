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
        [Authorize]
        public IActionResult Register([FromBody] Category category)
        {
            // Valida o modelo
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(category.Nome))
            {
                return BadRequest(new { mensagem = "Dados inválidos" });
            }

            // Tenta obter o email do usuário autenticado
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Se o token não contém informações válidas
            if (currentUserEmail == null)
            {
                return StatusCode(403, new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            // Busca o usuário no banco
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            // Verifica se o usuário é admin
            if (currentUser == null || currentUser.isAdmin != 1)
            {
                return StatusCode(403, new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            // Adiciona a categoria ao banco
            _context.Categories.Add(category);
            _context.SaveChanges();

            return StatusCode(201, new { mensagem = "Categoria criada com sucesso", categoria = category });
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Dados inválidos" });
            }

            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            if (currentUser == null || currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            var existingCategory = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            existingCategory.Nome = category.Nome;
            _context.SaveChanges();

            return Ok(new { mensagem = "Categoria atualizada com sucesso", categoria = existingCategory });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteCategory(int id)
        {
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var currentUser = _context.Users.SingleOrDefault(u => u.Email == currentUserEmail);

            if (currentUser == null || currentUser.isAdmin != 1)
            {
                return Unauthorized(new { mensagem = "Você não tem permissão suficiente para performar esta ação" });
            }

            var category = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { mensagem = "Categoria excluída com sucesso", id = category.Id });
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            // Recupera todas as categorias do banco de dados
            var categories = _context.Categories.Select(c => new
            {
                c.Id,
                c.Nome
            }).ToList();

            // Retorna as categorias no formato esperado
            return Ok(categories);
        }
    }
}
