using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nome { get; set; } = null!;

    public string? Sobrenome { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Rua { get; set; }

    public string? Morada { get; set; }

    public string? CodPostal { get; set; }

    public string? Genero { get; set; }

    public byte? Idade { get; set; }

    public DateTime? UltimoLogin { get; set; }

    public DateTime? DataCriacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public string? Status { get; set; }

    public bool? ReceberNewsletter { get; set; }

    public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();

    public virtual User NomeNavigation { get; set; } = null!;

    public virtual ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}
