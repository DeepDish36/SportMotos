using System;
using System.Collections.Generic;

namespace SportMotos.Models;

public partial class Imagem
{
    public int Id { get; set; } // Chave primária
    public string NomeArquivo { get; set; } = null!; // Nome do arquivo no servidor
    public string Caminho { get; set; } = null!; // Caminho completo da imagem (opcional)

    // Relacionamento - Uma imagem pode estar associada a uma Moto ou a uma Peça
    public int? MotoId { get; set; } // FK para Moto (pode ser nula)
    public virtual Moto Moto { get; set; } = null!;

    public int? PecaId { get; set; } // FK para Peça (pode ser nula)
    public virtual Peca Peca { get; set; } = null!;
}
