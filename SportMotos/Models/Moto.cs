using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Moto
{
    public int IdMoto { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Ano { get; set; }

    public int Quilometragem { get; set; }

    public double Preco { get; set; }

    public string Condicao { get; set; } = null!;

    public int Cilindrada { get; set; }

    public string? Cor { get; set; }

    public string Caixa { get; set; } = null!;

    public string Matricula { get; set; } = null!;

    public string Segmento { get; set; } = null!;

    public string Combustivel { get; set; } = null!;

    public string? Carta { get; set; }

    public bool Abs { get; set; }

    public string? Descricao { get; set; }

    public virtual List<Imagem> Imagens { get; set; } = new List<Imagem>(); // Nome do arquivo da imagem (ex: "AA-12-BC.jpg")

    public virtual ICollection<AnuncioMoto> AnuncioMotos { get; set; } = new List<AnuncioMoto>();

    // Propriedade de navegação para os interesses
    public virtual ICollection<InteresseMotos> Interesses { get; set; } = new List<InteresseMotos>();
}
