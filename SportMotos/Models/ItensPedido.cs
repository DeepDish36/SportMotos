using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class ItensPedido
    {
        public int IdItemPedido { get; set; }
        public int IdPedido { get; set; }  // FK para Pedido
        public int IdProduto { get; set; } // Pode ser Moto ou Peça
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        // Relacionamento com Pedido
        public virtual Pedidos Pedido { get; set; } = null!;
        public virtual Peca Peca { get; set; } = null!;
    }
}
