using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportMotos.Migrations
{
    /// <inheritdoc />
    public partial class corrigirfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Moto",
                columns: table => new
                {
                    ID_Moto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    Modelo = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Quilometragem = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Condicao = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    Cilindrada = table.Column<int>(type: "int", nullable: false),
                    Cor = table.Column<string>(type: "char(15)", unicode: false, fixedLength: true, maxLength: 15, nullable: true),
                    Caixa = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Matricula = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Segmento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Combustivel = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Carta = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    ABS = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Moto__7BA373CF34EB2800", x => x.ID_Moto);
                });

            migrationBuilder.CreateTable(
                name: "Noticia",
                columns: table => new
                {
                    ID_Noticia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data_Publicacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true),
                    Titulo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    SubTitulo = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Noticia__58E91D2061BD467F", x => x.ID_Noticia);
                });

            migrationBuilder.CreateTable(
                name: "Peca",
                columns: table => new
                {
                    ID_Peca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Categoria = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Marca = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    Modelo = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    Compatibilidade = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Data_Fabricacao = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Peso = table.Column<double>(type: "float", nullable: true),
                    Garantia = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Peca__B5902415D520233E", x => x.ID_Peca);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Tipo_Utilizador = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ultimo_Login = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__536C85E5DE2018A1", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Anuncio_Moto",
                columns: table => new
                {
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Moto = table.Column<int>(type: "int", nullable: false),
                    Vendido = table.Column<bool>(type: "bit", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Condicao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Data_Publicacao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data_Venda = table.Column<DateTime>(type: "datetime", nullable: true),
                    Visualizacoes = table.Column<int>(type: "int", nullable: true),
                    Favoritos = table.Column<int>(type: "int", nullable: true),
                    Avaliacoes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Anuncio___D8875FB6B9698F13", x => x.ID_Anuncio);
                    table.ForeignKey(
                        name: "FK__Anuncio_M__ID_Mo__66603565",
                        column: x => x.ID_Moto,
                        principalTable: "Moto",
                        principalColumn: "ID_Moto");
                });

            migrationBuilder.CreateTable(
                name: "Anuncio_Peca",
                columns: table => new
                {
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Peca = table.Column<int>(type: "int", nullable: false),
                    Vendido = table.Column<bool>(type: "bit", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Condicao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Data_Publicacao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data_Venda = table.Column<DateTime>(type: "datetime", nullable: true),
                    Visualizacoes = table.Column<int>(type: "int", nullable: true),
                    Favoritos = table.Column<int>(type: "int", nullable: true),
                    Avaliacoes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Anuncio___D8875FB67FD7777D", x => x.ID_Anuncio);
                    table.ForeignKey(
                        name: "FK__Anuncio_P__ID_Pe__6E01572D",
                        column: x => x.ID_Peca,
                        principalTable: "Peca",
                        principalColumn: "ID_Peca");
                });

            migrationBuilder.CreateTable(
                name: "Imagens",
                columns: table => new
                {
                    NomeArquivo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Caminho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotoId = table.Column<int>(type: "int", nullable: true),
                    PecaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imagens__7CCD9D8E31372040", x => x.NomeArquivo);
                    table.ForeignKey(
                        name: "FK_Imagens_Moto_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Moto",
                        principalColumn: "ID_Moto");
                    table.ForeignKey(
                        name: "FK_Imagens_Peca_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Peca",
                        principalColumn: "ID_Peca");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    ID_Admin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: false),
                    Telefone = table.Column<string>(type: "char(9)", unicode: false, fixedLength: true, maxLength: 9, nullable: true),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__69F567662E7796DA", x => x.ID_Admin);
                    table.ForeignKey(
                        name: "FK__Admin__Nome__3C69FB99",
                        column: x => x.Nome,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ID_Cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Sobrenome = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "char(9)", unicode: false, fixedLength: true, maxLength: 9, nullable: true),
                    Rua = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Morada = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CodPostal = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: true),
                    Genero = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    Idade = table.Column<byte>(type: "tinyint", nullable: true),
                    Ultimo_Login = table.Column<DateTime>(type: "datetime", nullable: true),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ReceberNewsletter = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cliente__E005FBFF59875125", x => x.ID_Cliente);
                    table.ForeignKey(
                        name: "FK__Cliente__Nome__5629CD9C",
                        column: x => x.Nome,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "VendaPeca",
                columns: table => new
                {
                    IdVenda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAnuncio = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendaPeca", x => x.IdVenda);
                    table.ForeignKey(
                        name: "FK_VendaPeca_Anuncio_Peca_IdAnuncio",
                        column: x => x.IdAnuncio,
                        principalTable: "Anuncio_Peca",
                        principalColumn: "ID_Anuncio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarrinhoCompras",
                columns: table => new
                {
                    ID_Carrinho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    ID_Peca = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DataAdicionado = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CarrinhoCompras__ID_Carrinho", x => x.ID_Carrinho);
                    table.ForeignKey(
                        name: "FK__CarrinhoCompras__ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                    table.ForeignKey(
                        name: "FK__CarrinhoCompras__ID_Peca",
                        column: x => x.ID_Peca,
                        principalTable: "Peca",
                        principalColumn: "ID_Peca");
                });

            migrationBuilder.CreateTable(
                name: "EnderecosEnvio",
                columns: table => new
                {
                    ID_Envio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Localidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RetiradaNaLoja = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnderecosEnvio", x => x.ID_Envio);
                    table.ForeignKey(
                        name: "FK_EnderecosEnvio_Cliente_ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Cliente = table.Column<int>(type: "int", nullable: false),
                    Id_Anuncio = table.Column<int>(type: "int", nullable: false),
                    TipoAnuncio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAdicionado = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Favoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoritos_AnuncioMotos",
                        column: x => x.Id_Anuncio,
                        principalTable: "Anuncio_Moto",
                        principalColumn: "ID_Anuncio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_AnuncioPecas",
                        column: x => x.Id_Anuncio,
                        principalTable: "Anuncio_Peca",
                        principalColumn: "ID_Anuncio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_Cliente",
                        column: x => x.Id_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                });

            migrationBuilder.CreateTable(
                name: "Forum",
                columns: table => new
                {
                    ID_Forum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: true),
                    ID_Admin = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    Data_Edicao = table.Column<DateTime>(type: "datetime", nullable: true),
                    Apagado_Em = table.Column<DateTime>(type: "datetime", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Ativo"),
                    Visualizacoes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Total_Respostas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Ultima_Resposta = table.Column<DateTime>(type: "datetime", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Likes = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forum", x => x.ID_Forum);
                    table.ForeignKey(
                        name: "FK_Forum_Admin",
                        column: x => x.ID_Admin,
                        principalTable: "Admin",
                        principalColumn: "ID_Admin");
                    table.ForeignKey(
                        name: "FK_Forum_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                });

            migrationBuilder.CreateTable(
                name: "InteresseMotos",
                columns: table => new
                {
                    ID_Interesse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    ID_Moto = table.Column<int>(type: "int", nullable: false),
                    DataInteresse = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "VARCHAR(20)", nullable: false, defaultValue: "Pendente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InteresseMotos__ID_Interesse", x => x.ID_Interesse);
                    table.ForeignKey(
                        name: "FK__InteresseMotos__ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__InteresseMotos__ID_Moto",
                        column: x => x.ID_Moto,
                        principalTable: "Moto",
                        principalColumn: "ID_Moto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orcamento",
                columns: table => new
                {
                    ID_Orcamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Valor_Total = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Pendente"),
                    Prazo_Resposta = table.Column<DateTime>(type: "datetime", nullable: true),
                    Notas_Administrador = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Metodo_Pagamento = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Ultima_Atualizacao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DetalhesVisualizados = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orcament__0503F9406E8490D9", x => x.ID_Orcamento);
                    table.ForeignKey(
                        name: "FK__Orcamento__ID_Cl__619B8048",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Cliente_ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    ID_Pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Cliente = table.Column<int>(type: "int", nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pendente"),
                    ClienteIdCliente = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedidos__ID_Pedido", x => x.ID_Pedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_Cliente_ClienteIdCliente",
                        column: x => x.ClienteIdCliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                    table.ForeignKey(
                        name: "FK__Pedidos__Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                });

            migrationBuilder.CreateTable(
                name: "Resposta",
                columns: table => new
                {
                    ID_Resposta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Forum = table.Column<int>(type: "int", nullable: false),
                    ID_Cliente = table.Column<int>(type: "int", nullable: true),
                    ID_Admin = table.Column<int>(type: "int", nullable: true),
                    Conteudo = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Data_Criacao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Resposta__69F567662E7796DA", x => x.ID_Resposta);
                    table.ForeignKey(
                        name: "FK__Resposta__ID_Admin",
                        column: x => x.ID_Admin,
                        principalTable: "Admin",
                        principalColumn: "ID_Admin");
                    table.ForeignKey(
                        name: "FK__Resposta__ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente");
                    table.ForeignKey(
                        name: "FK__Resposta__ID_Forum",
                        column: x => x.ID_Forum,
                        principalTable: "Forum",
                        principalColumn: "ID_Forum");
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoPeca",
                columns: table => new
                {
                    ID_OrcamentoPeca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Orcamento = table.Column<int>(type: "int", nullable: false),
                    ID_Peca = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoPeca", x => x.ID_OrcamentoPeca);
                    table.ForeignKey(
                        name: "FK_OrcamentoPeca_Orcamento",
                        column: x => x.ID_Orcamento,
                        principalTable: "Orcamento",
                        principalColumn: "ID_Orcamento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrcamentoPeca_Peca",
                        column: x => x.ID_Peca,
                        principalTable: "Peca",
                        principalColumn: "ID_Peca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensPedido",
                columns: table => new
                {
                    ID_ItemPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Pedido = table.Column<int>(type: "int", nullable: false),
                    ID_Peca = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPedido", x => x.ID_ItemPedido);
                    table.ForeignKey(
                        name: "FK_ItensPedido_Peca",
                        column: x => x.ID_Peca,
                        principalTable: "Peca",
                        principalColumn: "ID_Peca");
                    table.ForeignKey(
                        name: "FK_ItensPedido_Pedidos",
                        column: x => x.ID_Pedido,
                        principalTable: "Pedidos",
                        principalColumn: "ID_Pedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Admin__7D8FE3B2D8F144B3",
                table: "Admin",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_Moto_ID_Moto",
                table: "Anuncio_Moto",
                column: "ID_Moto");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_Peca_ID_Peca",
                table: "Anuncio_Peca",
                column: "ID_Peca");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoCompras_ID_Cliente",
                table: "CarrinhoCompras",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoCompras_ID_Peca",
                table: "CarrinhoCompras",
                column: "ID_Peca");

            migrationBuilder.CreateIndex(
                name: "UQ__Cliente__7D8FE3B2B0CEC7A9",
                table: "Cliente",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnderecosEnvio_ID_Cliente",
                table: "EnderecosEnvio",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_Id_Anuncio",
                table: "Favoritos",
                column: "Id_Anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_Id_Cliente",
                table: "Favoritos",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Forum_ID_Admin",
                table: "Forum",
                column: "ID_Admin");

            migrationBuilder.CreateIndex(
                name: "IX_Forum_ID_Cliente",
                table: "Forum",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_MotoId",
                table: "Imagens",
                column: "MotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_PecaId",
                table: "Imagens",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_InteresseMotos_ID_Cliente",
                table: "InteresseMotos",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_InteresseMotos_ID_Moto",
                table: "InteresseMotos",
                column: "ID_Moto");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_ID_Peca",
                table: "ItensPedido",
                column: "ID_Peca");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_ID_Pedido",
                table: "ItensPedido",
                column: "ID_Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_ID_Cliente",
                table: "Orcamento",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoPeca_ID_Orcamento",
                table: "OrcamentoPeca",
                column: "ID_Orcamento");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoPeca_ID_Peca",
                table: "OrcamentoPeca",
                column: "ID_Peca");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_ID_Cliente",
                table: "PasswordResets",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteIdCliente",
                table: "Pedidos",
                column: "ClienteIdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ID_Cliente",
                table: "Pedidos",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Resposta_ID_Admin",
                table: "Resposta",
                column: "ID_Admin");

            migrationBuilder.CreateIndex(
                name: "IX_Resposta_ID_Cliente",
                table: "Resposta",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Resposta_ID_Forum",
                table: "Resposta",
                column: "ID_Forum");

            migrationBuilder.CreateIndex(
                name: "IX_VendaPeca_IdAnuncio",
                table: "VendaPeca",
                column: "IdAnuncio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrinhoCompras");

            migrationBuilder.DropTable(
                name: "EnderecosEnvio");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropTable(
                name: "Imagens");

            migrationBuilder.DropTable(
                name: "InteresseMotos");

            migrationBuilder.DropTable(
                name: "ItensPedido");

            migrationBuilder.DropTable(
                name: "Noticia");

            migrationBuilder.DropTable(
                name: "OrcamentoPeca");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropTable(
                name: "Resposta");

            migrationBuilder.DropTable(
                name: "VendaPeca");

            migrationBuilder.DropTable(
                name: "Anuncio_Moto");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Orcamento");

            migrationBuilder.DropTable(
                name: "Forum");

            migrationBuilder.DropTable(
                name: "Anuncio_Peca");

            migrationBuilder.DropTable(
                name: "Moto");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Peca");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
