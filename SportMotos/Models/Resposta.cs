namespace SportMotos.Models
{
    public class Resposta
    {
        public int IdResposta { get; set; }
        public int IdForum { get; set; }
        public int? IdCliente { get; set; }
        public int? IdAdmin { get; set; } // Novo campo para administradores
        public string? Conteudo { get; set; }
        public DateTime DataCriacao { get; set; }

        // Propriedades de navegação
        public virtual Forum IdForumNavigation { get; set; }
        public virtual Cliente? IdClienteNavigation { get; set; }
        public virtual Admin? IdAdminNavigation { get; set; } // Novo relacionamento com Admin
    }
}
 
// Additional methods or properties can be added here if needed in the future.