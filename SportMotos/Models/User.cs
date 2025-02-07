using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string TipoUtilizador { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public DateTime UltimoLogin { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Cliente? Cliente { get; set; }
}
