using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    public string Nome { get; set; }
}
