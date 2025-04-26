namespace SportMotos.Models
{
    public class DetalhesOrcamentoViewModel
    {
        public Orcamento Orcamento { get; set; }
        public IEnumerable<Peca> PecasDisponiveis { get; set; }
        public List<int> PecasSelecionadas { get; set; } // IDs das peças selecionadas
        public Dictionary<int, int> Quantidades { get; set; } // Quantidade de cada peça
    }

}
