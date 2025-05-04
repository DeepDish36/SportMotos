using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models;

public partial class AnuncioPeca
{
    public int IdAnuncioPeca { get; set; }

    public int IdPeca { get; set; }

    public bool Vendido { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public double Preco { get; set; }

    public string Condicao { get; set; } = null!;

    public DateTime DataPublicacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public DateTime? DataExpiracao { get; set; }

    public DateTime? DataVenda { get; set; } // Novo campo

    public int? Visualizacoes { get; set; }

    public int? Favoritos { get; set; }

    public int? Avaliacoes { get; set; }

    public virtual Peca? IdPecaNavigation { get; set; }

    [NotMapped]
    public string? ImagemPath { get; set; }
}
