using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Forum
{
    public int IdForum { get; set; }

    public int IdCliente { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public string? Resposta { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
