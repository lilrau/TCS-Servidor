using System.ComponentModel.DataAnnotations;

namespace TCS_Cliente.Models
{
    public class NoticeRequest
    {
        [Required(ErrorMessage = "A descrição não pode estar em branco.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A categoria não pode estar em branco.")]
        public int CategoriaId { get; set; }
    }
}
