using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "O nome não pode estar em branco.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome invalido.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O nome não pode estar em branco.")]
        [EmailAddress(ErrorMessage = "Email invalido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha não pode estar em branco.")]
        [RegularExpression(@"^\d{3,6}$", ErrorMessage = "Senha invalida.")]
        public string Senha { get; set; }
    }
}
