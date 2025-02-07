using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Admin
{
    public int IdAdmin { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public DateTime? DataCriacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public virtual User NomeNavigation { get; set; } = null!;
}
