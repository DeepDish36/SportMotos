using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Orcamento
{
    public int IdOrcamento { get; set; } // ID único do orçamento

    public int IdCliente { get; set; } // Relacionado ao cliente

    public string Descricao { get; set; } = null!; // Detalhes do orçamento

    public DateTime DataCriacao { get; set; } // Data de criação do orçamento

    public double ValorTotal { get; set; } // Valor total do orçamento

    public string Status { get; set; } = "Pendente"; // Status atual do orçamento (Pendente, Aprovado, etc.)

    public DateTime? PrazoResposta { get; set; } // Prazo para resposta ao orçamento

    public string? NotasAdministrador { get; set; } // Notas adicionais do administrador (opcional)

    public string? MetodoPagamento { get; set; } // Método de pagamento proposto (opcional)

    public DateTime UltimaAtualizacao { get; set; } // Última atualização do orçamento

    // Relacionamento com Cliente
    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
