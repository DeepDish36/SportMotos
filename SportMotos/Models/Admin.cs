using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models;

public partial class Admin
{
    [Key]
    public int IdAdmin { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Digite um email válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = null!;

    public string? Telefone { get; set; } 

    public DateTime? DataCriacao { get; set; }

    public DateTime? DataEdicao { get; set; }

    public DateTime? ApagadoEm { get; set; }

    public virtual User? NomeNavigation { get; set; } = null!;
    public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
}
