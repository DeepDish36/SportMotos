using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class PasswordResets
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IDCliente { get; set; }

        public string? Token { get; set; }

        public DateTime Expiration {  get; set; }

        public virtual Cliente? Cliente { get; set; }
    }
}
