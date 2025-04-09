using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMotos.Models
{
    public class Favoritos
    {
        public int Id { get; set; }

        public int Id_Cliente { get; set; }

        public int AnuncioId { get; set; }

        public string TipoAnuncio { get; set; } // "motos" ou "pecas"

        public DateTime DataAdicionado { get; set; } = DateTime.Now;

        public virtual Cliente Cliente { get; set; } = null!;

        public virtual AnuncioMoto AnuncioMoto { get; set; }

        public virtual AnuncioPeca AnuncioPeca { get; set; }
    }
}
