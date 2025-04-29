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
        [ForeignKey("Peca")]
        public int IdPeca { get; set; } // 🔥 Agora referencia diretamente a peça!

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; } = 1;

        public DateTime? DataAdicionado { get; set; } = DateTime.Now;

        // 🔥 Propriedades de Navegação
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Peca Peca { get; set; } = null!; // Referência correta à tabela Peca
    }
}
