using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Dados inválidos.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Dados inválidos.")]
        public string Nome { get; set; }
    }
}
