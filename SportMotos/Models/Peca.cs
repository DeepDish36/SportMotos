using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Peca
{
    public int IdPeca { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string? Modelo { get; set; }

    public string? Compatibilidade { get; set; }

    public double Preco { get; set; }

    public int DataFabricacao { get; set; }

    public int Stock { get; set; }

    public string Estado { get; set; } = null!;

    public double? Peso { get; set; }

    public byte? Garantia { get; set; }

    public virtual List<Imagem> Imagens { get; set; } = new List<Imagem>(); 

    public virtual ICollection<AnuncioPeca> AnuncioPecas { get; set; } = new List<AnuncioPeca>();
    public virtual ICollection<OrcamentoPeca> OrcamentoPecas { get; set; } = new List<OrcamentoPeca>();
    public virtual ICollection<CarrinhoCompras> CarrinhoCompras { get; set; } = new List<CarrinhoCompras>();

}
