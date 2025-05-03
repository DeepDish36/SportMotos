using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class EnderecosEnvio
    {
        [Key]
        [Column("ID_Envio")]
        public int IdEnvio { get; set; }

        public int IdCliente { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Apelido { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Telefone { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Localidade { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string CodigoPostal { get; set; } = null!;

        [Column("RetiradaNaLoja")]
        public bool RetiradaNaLoja { get; set; } = false;

        // Propriedade de navegação
        public virtual Cliente Cliente { get; set; } = null!;
    }
}
