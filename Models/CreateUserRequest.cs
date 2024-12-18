using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Dados invalidos.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Dados invalidos.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Dados invalidos.")]
        [EmailAddress(ErrorMessage = "Dados invalidos.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Dados invalidos.")]
        [RegularExpression(@"^\d{3,6}$", ErrorMessage = "Dados invalidos.")]
        public string Senha { get; set; }
    }
}
