using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using TCS_Cliente.Data;
using TCS_Cliente.Models;

namespace TCS_Cliente.Controllers
{
    [Route("avisos")]
    [ApiController]
    public class NoticesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NoticesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateNotice([FromBody] Notice notice)
        {
            // Valida o modelo
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(notice.Descricao) || notice.CategoriaId <= 0)
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

            // Verifica se a categoria existe
            var category = _context.Categories.SingleOrDefault(c => c.Id == notice.CategoriaId);
            if (category == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            // Adiciona o aviso ao banco
            _context.Notices.Add(notice);
            _context.SaveChanges();

            return StatusCode(201, new { mensagem = "Aviso criado com sucesso", aviso = notice });
        }

        [HttpGet]
        public IActionResult GetAllNotices()
        {
            // Recupera todos os avisos do banco de dados
            var notices = _context.Notices
                .Select(n => new
                {
                    n.Id,
                    n.Descricao,
                    n.CategoriaId
                }).ToList();

            return Ok(notices);
        }

        [HttpGet("{idCategoria}")]
        public IActionResult GetNoticesByCategory(int idCategoria)
        {
            // Verifica se a categoria existe
            var category = _context.Categories.SingleOrDefault(c => c.Id == idCategoria);
            if (category == null)
            {
                return NotFound(new { mensagem = "Categoria não encontrada" });
            }

            // Recupera os avisos da categoria
            var notices = _context.Notices
                .Where(n => n.CategoriaId == idCategoria)
                .Select(n => new
                {
                    n.Id,
                    n.Descricao
                }).ToList();

            return Ok(notices);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateNotice(int id, [FromBody] Notice notice)
        {
            // Valida o modelo
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(notice.Descricao))
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

            // Busca o aviso no banco
            var existingNotice = _context.Notices.SingleOrDefault(n => n.Id == id);
            if (existingNotice == null)
            {
                return NotFound(new { mensagem = "Aviso não encontrado" });
            }

            // Atualiza a descrição do aviso
            existingNotice.Descricao = notice.Descricao;
            _context.SaveChanges();

            return Ok(new { mensagem = "Aviso atualizado com sucesso", aviso = existingNotice });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteNotice(int id)
        {
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

            // Busca o aviso no banco
            var notice = _context.Notices.SingleOrDefault(n => n.Id == id);
            if (notice == null)
            {
                return NotFound(new { mensagem = "Aviso não encontrado" });
            }

            // Remove o aviso do banco
            _context.Notices.Remove(notice);
            _context.SaveChanges();

            return Ok(new { mensagem = "Aviso excluído com sucesso", id = notice.Id });
        }
    }
}