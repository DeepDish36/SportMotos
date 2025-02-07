using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Orcamento
{
    public int IdForum { get; set; }

    public int IdCliente { get; set; }

    public string Descricao { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public double ValorTotal { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
