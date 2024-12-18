namespace TCS_Cliente.Models
{
    public class User
    {
        public int Id { get; set; }  // autoincremental pelo EF Core
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public int isAdmin { get; set; }
    }
}
