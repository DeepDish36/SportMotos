using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class VendaPeca
    {
        [Key]
        public int IdVenda { get; set; }

        [Required]
        public int IdAnuncio { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [Required]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        // Relacionamento com AnuncioPeca
        [ForeignKey("IdAnuncio")]
        public virtual AnuncioPeca? AnuncioPeca { get; set; }
    }
}
