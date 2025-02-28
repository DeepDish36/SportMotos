using SportMotos.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Pedidos
{
    [Key]
    public int IdPedido { get; set; }

    [ForeignKey("Cliente")]
    [Column("Id_Cliente")]
    public int IdCliente { get; set; }  // Chave estrangeira para Cliente

    [Column("DataCompra")]
    public DateTime DataCompra { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pendente";

    // Relacionamento com Cliente (Muitos para Um)
    public virtual Cliente Cliente { get; set; } = null!;

    // Relacionamento com ItensPedido (Um Pedido tem vários Itens)
    public virtual ICollection<ItensPedido> Itens { get; set; } = new List<ItensPedido>();
}
