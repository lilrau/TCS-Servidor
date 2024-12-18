using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class UserUpdateRequest
    {
        [RegularExpression(@"^\d{3,6}$", ErrorMessage = "A senha deve conter entre 3 e 6 dígitos numéricos")]
        public string? Senha { get; set; }

        public string? Nome { get; set; }
    }
}
