using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Noticium
{
    public int IdNoticia { get; set; }

    public DateTime DataPublicacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public string Titulo { get; set; } = null!;

    public string? SubTitulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

}
