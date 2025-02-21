using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportMotos.Models;

public partial class Cliente
{
    [Key]  // Define como chave primária
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = null!;

    public string? Sobrenome { get; set; }

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Digite um email válido")]
    public string Email { get; set; }= null!;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Rua { get; set; }

    public string? Morada { get; set; }

    public string? CodPostal { get; set; }

    public string? Genero { get; set; }

    public byte? Idade { get; set; }

    public DateTime? UltimoLogin { get; set; } = DateTime.Now;

    public DateTime? DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataEdicao { get; set; } = DateTime.Now;

    public DateTime? ApagadoEm { get; set; }

    public string? Status { get; set; }

    public bool? ReceberNewsletter { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiration { get; set; }

    public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();

    public virtual User? NomeNavigation { get; set; } = null!;

    public virtual ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}
