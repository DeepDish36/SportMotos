﻿using System;
using System.Collections.Generic;
using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SportMotos.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AnuncioMoto> AnuncioMotos { get; set; }

    public virtual DbSet<AnuncioPeca> AnuncioPecas { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Forum> Forums { get; set; }

    public virtual DbSet<Favoritos> Favoritos { get; set; }

    public virtual DbSet<Imagem> Imagens { get; set; }

    public virtual DbSet<Moto> Motos { get; set; }

    public virtual DbSet<Noticium> Noticia { get; set; }

    public virtual DbSet<Orcamento> Orcamentos { get; set; }

    public virtual DbSet<Peca> Pecas { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<PasswordResets> PasswordResets { get; set; }

    public virtual DbSet<Pedidos> Pedidos { get; set; }

    public virtual DbSet<Resposta> Resposta { get; set; }

    public virtual DbSet<InteresseMotos> InteresseMotos { get; set; }

    public virtual DbSet<CarrinhoCompras> CarrinhoCompras { get; set; }

    public virtual DbSet<OrcamentoPeca> OrcamentoPeca { get; set; } // Adicionando a DbSet para OrcamentoPeca

    public virtual DbSet<ItensPedido> ItensPedido { get; set; } // Adicionando a DbSet para ItensPedido

    public virtual DbSet<EnderecosEnvio> EnderecosEnvios { get; set; } // Adicionando a DbSet para EnderecosEnvio

    public virtual DbSet<VendaPeca> VendaPeca { get; set; } // Adicionando a DbSet para VendaPeca

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
        .UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|SportMotos.mdf;Integrated Security=True;Connect Timeout=30")
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__Admin__69F567662E7796DA");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Nome, "UQ__Admin__7D8FE3B2D8F144B3").IsUnique();

            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");
            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");
            entity.Property(e => e.DataCriacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");
            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(90)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.NomeNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.Nome)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__Nome__3C69FB99");
        });

        modelBuilder.Entity<AnuncioMoto>(entity =>
        {
            entity.HasKey(e => e.IdAnuncioMoto).HasName("PK__Anuncio___D8875FB6B9698F13");

            entity.ToTable("Anuncio_Moto");

            entity.Property(e => e.IdAnuncioMoto).HasColumnName("ID_Anuncio");
            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");
            entity.Property(e => e.Condicao)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");
            entity.Property(e => e.DataPublicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Publicacao");
            entity.Property(e => e.DataVenda)
                .HasColumnType("datetime")
                .HasColumnName("Data_Venda");
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.IdMoto).HasColumnName("ID_Moto");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMotoNavigation).WithMany(p => p.AnuncioMotos)
                .HasForeignKey(d => d.IdMoto)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Anuncio_M__ID_Mo__66603565");
        });

        modelBuilder.Entity<AnuncioPeca>(entity =>
        {
            entity.HasKey(e => e.IdAnuncioPeca).HasName("PK__Anuncio___D8875FB67FD7777D");

            entity.ToTable("Anuncio_Peca");

            entity.Property(e => e.IdAnuncioPeca).HasColumnName("ID_Anuncio");
            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");
            entity.Property(e => e.Condicao)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");
            entity.Property(e => e.DataPublicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Publicacao");
            entity.Property(e => e.DataVenda)
                .HasColumnType("datetime")
                .HasColumnName("Data_Venda");
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.IdPeca).HasColumnName("ID_Peca");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPecaNavigation).WithMany(p => p.AnuncioPecas)
                .HasForeignKey(d => d.IdPeca)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Anuncio_P__ID_Pe__6E01572D");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__E005FBFF59875125");

            entity.ToTable("Cliente");

            entity.HasIndex(e => e.Nome, "UQ__Cliente__7D8FE3B2B0CEC7A9").IsUnique();

            entity.Property(e => e.IdCliente).HasColumnName("ID_Cliente");
            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");
            entity.Property(e => e.CodPostal)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.DataCriacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");
            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Genero)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Morada).IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ReceberNewsletter).HasDefaultValue(false);
            entity.Property(e => e.Rua)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UltimoLogin)
                .HasColumnType("datetime")
                .HasColumnName("Ultimo_Login");

            entity.HasOne(d => d.NomeNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.Nome)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cliente__Nome__5629CD9C");
        });

        modelBuilder.Entity<Favoritos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favoritos");

            entity.ToTable("Favoritos");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Id_Cliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.Id_Anuncio).HasColumnName("Id_Anuncio");
            entity.Property(e => e.TipoAnuncio).HasColumnName("TipoAnuncio");
            entity.Property(e => e.DataAdicionado).HasColumnName("DataAdicionado").HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.Id_Cliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favoritos_Cliente");

            entity.HasOne(d => d.AnuncioMoto)
                .WithMany()
                .HasForeignKey(d => d.Id_Anuncio)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Favoritos_AnuncioMotos");

            entity.HasOne(d => d.AnuncioPeca)
                .WithMany()
                .HasForeignKey(d => d.Id_Anuncio)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Favoritos_AnuncioPecas");
        });

        modelBuilder.Entity<Forum>(entity =>
        {
            entity.HasKey(e => e.IdForum).HasName("PK_Forum");

            entity.ToTable("Forum");

            entity.Property(e => e.IdForum).HasColumnName("ID_Forum");

            entity.Property(e => e.IdCliente).HasColumnName("ID_Cliente").IsRequired(false); // Agora permite NULL
            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin").IsRequired(false); // Novo campo para admins

            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.Descricao)
                .IsUnicode(false);

            entity.Property(e => e.DataCriacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");

            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");

            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("Estado")
                .HasDefaultValue("Ativo");

            entity.Property(e => e.Visualizacoes)
                .HasColumnName("Visualizacoes")
                .HasDefaultValue(0);

            entity.Property(e => e.TotalRespostas)
                .HasColumnName("Total_Respostas")
                .HasDefaultValue(0);

            entity.Property(e => e.UltimaResposta)
                .HasColumnType("datetime")
                .HasColumnName("Ultima_Resposta");

            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .HasColumnName("Categoria");

            entity.Property(e => e.Likes)
                .HasColumnName("Likes")
                .HasDefaultValue(0);

            // 🔗 Relacionamento com Cliente
            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Forums)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Forum_Cliente");

            // 🔗 Relacionamento com Admin
            entity.HasOne(d => d.IdAdminNavigation).WithMany(a => a.Forums)
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Forum_Admin");
        });

        modelBuilder.Entity<Imagem>(entity =>
        {
            entity.HasKey(e => e.NomeArquivo).HasName("PK__Imagens__7CCD9D8E31372040");

            entity.Property(e => e.NomeArquivo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("NomeArquivo");
        });

        modelBuilder.Entity<Moto>(entity =>
        {
            entity.HasKey(e => e.IdMoto).HasName("PK__Moto__7BA373CF34EB2800");

            entity.ToTable("Moto");

            entity.Property(e => e.IdMoto).HasColumnName("ID_Moto");
            entity.Property(e => e.Abs).HasColumnName("ABS");
            entity.Property(e => e.Caixa)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Carta)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Combustivel)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Condicao)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Cor)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Matricula)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Segmento)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Noticium>(entity =>
        {
            entity.HasKey(e => e.IdNoticia).HasName("PK__Noticia__58E91D2061BD467F");

            entity.Property(e => e.IdNoticia).HasColumnName("ID_Noticia");
            entity.Property(e => e.ApagadoEm)
                .HasColumnType("datetime")
                .HasColumnName("Apagado_Em");
            entity.Property(e => e.DataEdicao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Edicao");
            entity.Property(e => e.DataPublicacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Publicacao");
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SubTitulo)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Orcamento>(entity =>
        {
            entity.HasKey(e => e.IdOrcamento).HasName("PK__Orcament__0503F9406E8490D9");

            entity.ToTable("Orcamento");

            // Configurações de propriedades
            entity.Property(e => e.IdOrcamento)
                .HasColumnName("ID_Orcamento"); // Corrigido para o nome correto da coluna

            entity.Property(e => e.IdCliente)
                .HasColumnName("ID_Cliente");

            entity.Property(e => e.Descricao)
                .IsUnicode(false)
                .HasColumnName("Descricao");

            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");

            entity.Property(e => e.ValorTotal)
                .HasColumnName("Valor_Total");

            entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasDefaultValue("Pendente"); // Define valor padrão

            entity.Property(e => e.PrazoResposta)
                .HasColumnName("Prazo_Resposta")
                .HasColumnType("datetime");

            entity.Property(e => e.NotasAdministrador)
                .HasColumnName("Notas_Administrador")
                .IsUnicode(false);

            entity.Property(e => e.MetodoPagamento)
                .HasColumnName("Metodo_Pagamento")
                .IsUnicode(false);

            entity.Property(e => e.UltimaAtualizacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Ultima_Atualizacao");

            // Configuração da chave estrangeira
            entity.HasOne(d => d.IdClienteNavigation)
                .WithMany(p => p.Orcamentos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orcamento__ID_Cl__619B8048");
        });

        modelBuilder.Entity<Peca>(entity =>
        {
            entity.HasKey(e => e.IdPeca).HasName("PK__Peca__B5902415D520233E");

            entity.ToTable("Peca");

            entity.Property(e => e.IdPeca).HasColumnName("ID_Peca");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Compatibilidade).IsUnicode(false);
            entity.Property(e => e.DataFabricacao).HasColumnName("Data_Fabricacao");
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Users__536C85E5DE2018A1");

            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Data_Criacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Tipo_Utilizador)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Tipo_Utilizador");
            entity.Property(e => e.Ultimo_Login)
                .HasColumnType("datetime")
                .HasColumnName("Ultimo_Login");
        });

        modelBuilder.Entity<PasswordResets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PasswordResets");

            entity.ToTable("PasswordResets");

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Expiration)
                .HasColumnType("datetime");

            entity.Property(e => e.IdCliente)
                .HasColumnName("ID_Cliente")
                .IsRequired();
        });

        modelBuilder.Entity<InteresseMotos>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.IdInteresse).HasName("PK__InteresseMotos__ID_Interesse");

            // Nome da tabela
            entity.ToTable("InteresseMotos");

            // Propriedades
            entity.Property(e => e.IdInteresse).HasColumnName("ID_Interesse");
            entity.Property(e => e.DataInteresse)
                .HasColumnType("datetime")
                .HasColumnName("DataInteresse");
            entity.Property(e => e.Status)
                .HasColumnType("VARCHAR(20)")
                .HasDefaultValue("Pendente")
                .HasColumnName("Status");

            // Configuração das chaves estrangeiras
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Interesses) // Assume que o cliente pode ter múltiplos interesses
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Cascade) // Define comportamento de deleção em cascata
                .HasConstraintName("FK__InteresseMotos__ID_Cliente");

            entity.HasOne(e => e.Moto)
                .WithMany(m => m.Interesses) // Assume que a moto pode ter múltiplos interesses
                .HasForeignKey(e => e.IdMoto)
                .OnDelete(DeleteBehavior.Cascade) // Define comportamento de deleção em cascata
                .HasConstraintName("FK__InteresseMotos__ID_Moto");
        });

        modelBuilder.Entity<Pedidos>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__Pedidos__ID_Pedido");

            entity.ToTable("Pedidos");

            entity.Property(e => e.IdPedido).HasColumnName("ID_Pedido");
            entity.Property(e => e.DataCompra)
                .HasColumnType("datetime")
                .HasColumnName("DataCompra")
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10,2)")
                .IsRequired();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Status")
                .HasDefaultValue("Pendente");

            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.IdCliente)
                .HasPrincipalKey(c => c.IdCliente);
        });

        modelBuilder.Entity<CarrinhoCompras>(entity =>
        {
            entity.HasKey(e => e.IdCarrinho).HasName("PK__CarrinhoCompras__ID_Carrinho");

            entity.ToTable("CarrinhoCompras");

            entity.Property(e => e.IdCarrinho).HasColumnName("ID_Carrinho");
            entity.Property(e => e.IdCliente).HasColumnName("ID_Cliente").IsRequired();
            entity.Property(e => e.IdPeca).HasColumnName("ID_Peca").IsRequired();
            entity.Property(e => e.Quantidade).HasColumnName("Quantidade")
                .IsRequired()
                .HasDefaultValue(1);
            entity.Property(e => e.DataAdicionado)
                .HasColumnType("datetime")
                .HasColumnName("DataAdicionado")
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Cliente)
                .WithMany(p => p.CarrinhoCompras)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarrinhoCompras__ID_Cliente");

            entity.HasOne(d => d.Peca)
                .WithMany(p => p.CarrinhoCompras)
                .HasForeignKey(d => d.IdPeca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarrinhoCompras__ID_Peca");
        });

        modelBuilder.Entity<Resposta>(entity =>
        {
            entity.HasKey(e => e.IdResposta).HasName("PK__Resposta__69F567662E7796DA");

            entity.ToTable("Resposta");

            entity.Property(e => e.IdResposta).HasColumnName("ID_Resposta");
            entity.Property(e => e.IdForum).HasColumnName("ID_Forum");
            entity.Property(e => e.IdCliente).HasColumnName("ID_Cliente").IsRequired(false);
            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin").IsRequired(false); // Novo campo para admins
            entity.Property(e => e.Conteudo).IsUnicode(false);
            entity.Property(e => e.DataCriacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");

            entity.HasOne(d => d.IdForumNavigation).WithMany(p => p.Respostas)
                .HasForeignKey(d => d.IdForum)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resposta__ID_Forum");

            entity.HasOne(d => d.IdClienteNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resposta__ID_Cliente");

            entity.HasOne(d => d.IdAdminNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resposta__ID_Admin");
        });

        modelBuilder.Entity<ItensPedido>(entity =>
        {
            entity.HasKey(e => e.IdItemPedido).HasName("PK_ItensPedido");

            entity.ToTable("ItensPedido");

            entity.Property(e => e.IdItemPedido).HasColumnName("ID_ItemPedido");
            entity.Property(e => e.IdPedido).IsRequired();
            entity.Property(e => e.IdPeca).IsRequired();
            entity.Property(e => e.Quantidade)
                .IsRequired()
                .HasDefaultValue(1);
            entity.Property(e => e.PrecoUnitario)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            // 🔥 Relação com Pedidos
            entity.HasOne(d => d.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ItensPedido_Pedidos");

            // 🔥 Relação com Peças
            entity.HasOne(d => d.Peca)
                .WithMany(p => p.Itens)
                .HasForeignKey(d => d.IdPeca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItensPedido_Peca");
        });

        Console.WriteLine("Configurando entidade OrcamentoPeca...");
        modelBuilder.Entity<OrcamentoPeca>(entity =>
        {
            entity.HasKey(e => e.IdOrcamentoPeca).HasName("PK_OrcamentoPeca");

            entity.Property(e => e.IdOrcamentoPeca).HasColumnName("ID_OrcamentoPeca");
            entity.Property(e => e.IdOrcamento).HasColumnName("ID_Orcamento");
            entity.Property(e => e.IdPeca).HasColumnName("ID_Peca");
            entity.Property(e => e.Quantidade).HasColumnName("Quantidade");

            // Mapeamento correto para a FK de Orçamento
            entity.HasOne(op => op.IdOrcamentoNavigation)
                    .WithMany(o => o.OrcamentoPecas) // Um orçamento pode conter várias peças
                    .HasForeignKey(op => op.IdOrcamento)
                    .HasConstraintName("FK_OrcamentoPeca_Orcamento");

            // Mapeamento correto para a FK de Peça (corrigido para não duplicar)
            entity.HasOne(op => op.IdPecaNavigation)
                    .WithMany(p => p.OrcamentoPecas) // Uma peça pode estar em vários orçamentos
                    .HasForeignKey(op => op.IdPeca)
                    .HasConstraintName("FK_OrcamentoPeca_Peca");
        });

        Console.WriteLine("Configuração de OrcamentoPeca concluída.");

        modelBuilder.Entity<EnderecosEnvio>(entity =>
        {
            entity.ToTable("EnderecosEnvio");

            entity.HasKey(e => e.IdEnvio);

            entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apelido).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telefone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Localidade).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cidade).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CodigoPostal).HasMaxLength(10).IsRequired();
            entity.Property(e => e.RetiradaNaLoja).HasDefaultValue(false);

            entity.Property(e => e.IdCliente)
                  .HasColumnName("ID_Cliente");

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.EnderecosEnvio)
                  .HasForeignKey(e => e.IdCliente)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VendaPeca>()
            .HasOne(v => v.AnuncioPeca)
            .WithMany()
            .HasForeignKey(v => v.IdAnuncio)
            .OnDelete(DeleteBehavior.Cascade); // Apaga as vendas ao excluir um anúncio

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
