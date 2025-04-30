using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class CarrinhoCompras
    {
        [Key]
        [Column("ID_Carrinho")]
        public int IdCarrinho { get; set; }

        [ForeignKey("Cliente")]
        [Column("ID_Cliente")]
        public int IdCliente { get; set; }

        [ForeignKey("Peca")]
        [Column("ID_Peca")]
        public int IdPeca { get; set; } // 🔥 Agora referencia corretamente a peça!

        [Required]
        public int Quantidade { get; set; } = 1;

        [Column("DataAdicionado")]
        public DateTime? DataAdicionado { get; set; } = DateTime.Now;

        // 🔥 Propriedades de Navegação
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Peca Peca { get; set; } = null!;
    }
}
