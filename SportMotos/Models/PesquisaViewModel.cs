namespace SportMotos.Models
{
    public class PesquisaViewModel
    {
        public List<Moto> Motos { get; set; } = new List<Moto>();
        public List<Peca> Pecas { get; set; } = new List<Peca>();
        public List<Noticium> Noticias { get; set; } = new List<Noticium>();
        public List<Forum> Foruns { get; set; } = new List<Forum>();
    }
}
