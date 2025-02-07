using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models
{
    public class Admin
    {
        [Key]

        [Required]
        public int ID_Admin { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Telefone { get; set; }

        public DateTime Data_Criacao { get; set; } = DateTime.Now;

        public DateTime Data_Edicao { get; set; } = DateTime.Now;

        public DateTime Apagado_Em { get; set; } = DateTime.Now;
    }
}
