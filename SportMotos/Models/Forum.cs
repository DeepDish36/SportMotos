using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Forum
{
    public int IdForum { get; set; }

    public int? IdCliente { get; set; } // Agora pode ser NULL, pois admins podem criar fóruns

    public int? IdAdmin { get; set; } // Novo campo para administradores

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public string Estado { get; set; } = "Ativo"; // Estado do fórum (ex: Ativo, Encerrado)

    public int Visualizacoes { get; set; } = 0; // Contagem de visualizações

    public int TotalRespostas { get; set; } = 0; // Número total de respostas

    public DateTime? UltimaResposta { get; set; } // Data da última resposta

    public string? Categoria { get; set; } // Categoria do fórum (Sugestões, Discussões, Dúvidas, etc.)

    public int Likes { get; set; } = 0; // Curtidas no fórum

    // Relacionamentos com Cliente e Admin
    public virtual Cliente? IdClienteNavigation { get; set; }
    public virtual Admin? IdAdminNavigation { get; set; } // Novo relacionamento com Admin

    public virtual ICollection<Resposta>? Respostas { get; set; }
}
