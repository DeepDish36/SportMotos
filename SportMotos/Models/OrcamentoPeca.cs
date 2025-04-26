using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class OrcamentoPeca
    {
        public int IdOrcamentoPeca { get; set; } // Chave primária

        public int IdOrcamento { get; set; } // Chave estrangeira para Orcamento
        public int IdPeca { get; set; } // Chave estrangeira para Peca

        public int Quantidade { get; set; } // Quantidade da peça no orçamento

        // Relacionamentos
        public virtual Orcamento Orcamento { get; set; } // Relacionamento com Orcamento
        public virtual Peca Peca { get; set; } // Relacionamento com Peca
    }
}
