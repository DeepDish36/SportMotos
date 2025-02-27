using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class CarrinhoCompras
    {
        [Key]
        [Column("ID_Carrinho")]
        public int IdCarrinho { get; set; }

        [Required]
        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        [Required]
        public int IdProduto { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string TipoProduto { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; } = 1;

        public DateTime? DataAdicionado { get; set; } = DateTime.Now;

        // Propriedade de Navegação
        public virtual Cliente Cliente { get; set; } = null!;
    }
}
