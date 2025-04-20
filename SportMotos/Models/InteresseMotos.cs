using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class InteresseMotos
    {
        [Key]
        [Column("ID_Interesse")]
        public int IdInteresse { get; set; }

        [Required]
        [Column("ID_Cliente")] // Corrige o mapeamento para a coluna correta
        public int IdCliente { get; set; }

        [Required]
        [Column("ID_Moto")] // Corrige o mapeamento para a coluna correta
        public int IdMoto { get; set; }

        [Column("DataInteresse")] // Corrige o nome para corresponder ao banco de dados
        public DateTime? DataInteresse { get; set; } = DateTime.Now;

        [Column("Status", TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "Pendente";

        // Propriedades de Navegação (não mapeadas para colunas)
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdMoto")]
        public virtual Moto? Moto { get; set; }
    }
}
