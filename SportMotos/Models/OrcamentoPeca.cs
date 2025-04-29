using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class OrcamentoPeca
    {
        public int IdOrcamentoPeca { get; set; }
        public int IdOrcamento { get; set; }
        public int IdPeca { get; set; }
        public int Quantidade { get; set; }

        public virtual Orcamento? IdOrcamentoNavigation { get; set; }
        public virtual Peca? IdPecaNavigation { get; set; }
    }
}
