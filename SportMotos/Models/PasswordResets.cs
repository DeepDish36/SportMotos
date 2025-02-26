using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class PasswordResets
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdCliente { get; set; }

        public string? Token { get; set; }

        public DateTime Expiration {  get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }
    }
}
