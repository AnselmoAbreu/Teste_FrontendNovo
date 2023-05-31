using System.ComponentModel.DataAnnotations;

namespace Teste_Frontend.Models
{
    public class Produto
    {
        [Key]
        [Display(Name = "Id")]

        public int id { get; set; }

        public string Nome { get; set; }

        [Display(Name = "Descrição")]

        public string Descricao { get; set; }

        [Display(Name = "Preço")]

        public decimal Preco { get; set; }

        public int Estoque { get; set; }
    }
}
