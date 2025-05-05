using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class ItensPedido
    {
        [Key]
        [Column("ID_ItemPedido")]
        public int IdItemPedido { get; set; }

        [Required]
        [ForeignKey("Pedido")]
        [Column("ID_Pedido")]
        public int IdPedido { get; set; } // FK para Pedido

        [Required]
        [ForeignKey("Peca")]
        [Column("ID_Peca")]
        public int IdPeca { get; set; } // FK para Peça

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        // 🔥 Propriedades de navegação
        public virtual Pedidos Pedido { get; set; } = null!;
        public virtual Peca Peca { get; set; } = null!;
    }
}
