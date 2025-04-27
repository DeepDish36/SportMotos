using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class OrcamentoPeca
    {
        [Key]
        [Column("ID_OrcamentoPeca")]
        public int IdOrcamentoPeca { get; set; } // Chave primária

        [Column("ID_Orcamento")]
        public int IdOrcamento { get; set; } // FK para Orcamento

        [Column("ID_Peca")]
        public int IdPeca { get; set; } // FK para Peca

        [Column("Quantidade")]
        public int Quantidade { get; set; }

        // Relacionamentos

        [ForeignKey(nameof(IdOrcamento))]
        public virtual Orcamento Orcamento { get; set; }

        [ForeignKey(nameof(IdPeca))]
        public virtual Peca Peca { get; set; }
    }
}
