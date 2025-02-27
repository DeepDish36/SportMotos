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
        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        [Required]
        [ForeignKey("Moto")]
        public int IdMoto { get; set; }

        public DateTime? DataInteresse { get; set; } = DateTime.Now;

        [Column(TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "Pendente";

        // Propriedades de Navegação
        public virtual Cliente Cliente { get; set; } = null!;

        public virtual Moto Moto { get; set; } = null!;
    }

}
