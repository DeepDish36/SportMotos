namespace SportMotos.Models
{
    public class OrcamentoViewModel
    {
        public int IdCliente { get; set; } // ID do cliente associado ao orçamento
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Servico { get; set; }
        public string Descricao { get; set; }
        public DateTime? PrazoResposta { get; set; }
        public string? MetodoPagamento { get; set; }
    }
}
