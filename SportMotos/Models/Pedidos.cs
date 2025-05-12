using SportMotos.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models;

[Table("Pedidos")]
public class Pedidos
{
    [Key]
    [Column("ID_Pedido")]
    public int IdPedido { get; set; }

    [Required]
    [Column("ID_Cliente")]
    public int IdCliente { get; set; }
    public DateTime DataCompra { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pendente";

    // Relacionamento com ItensPedido (Um Pedido tem vários Itens)
    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<ItensPedido> Itens { get; set; } = new List<ItensPedido>();
}
