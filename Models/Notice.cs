using System.Text.Json.Serialization;

namespace TCS_Cliente.Models
{
    public class Notice
    {
        public int Id { get; set; }
        public string Descricao { get; set; }    

        [JsonPropertyName("idCategoria")]
        public int CategoriaId { get; set; }
    }
}
