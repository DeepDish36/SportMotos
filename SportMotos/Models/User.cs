using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Tipo_Utilizador { get; set; } = null!;

    public DateTime Data_Criacao { get; set; }

    public DateTime Ultimo_Login { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Cliente? Cliente { get; set; }
}
