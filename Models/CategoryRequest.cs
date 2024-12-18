using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Dados inválidos.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Dados inválidos.")]
        public string Nome { get; set; }
    }
}
