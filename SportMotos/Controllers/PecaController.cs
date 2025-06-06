﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class PecaController : Controller
    {
        //Controller para criar uma moto
        private readonly AppDbContext _context;

        public PecaController(AppDbContext context)
        {
            _context = context;
        }

        //Listar peças
        public async Task<IActionResult> ListarPecas()
        {
            var pecas = await _context.Pecas.ToListAsync();
            return View(pecas);
        }

        //Detalhes da peça
        public async Task<IActionResult> DetalhesPeca(int id)
        {
            var pecas = await _context.Pecas.FindAsync(id);
            if (pecas == null) return NotFound();
            return View(pecas);
        }

        //Formulário de adição da peça ao sistema
        [HttpGet]
        public IActionResult AdicionarPeca()
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Adicionar peça ao sistema (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarPeca(Peca peca, List<IFormFile> Imagens)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(peca);
            }

            // Adiciona a peça ao banco de dados
            _context.Pecas.Add(peca);
            await _context.SaveChangesAsync(); // Salva primeiro para gerar o ID

            // Processa as imagens
            if (Imagens != null && Imagens.Count > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pecas");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var imagem in Imagens)
                {
                    if (imagem.Length > 0)
                    {
                        string fileName = $"{peca.IdPeca}_{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(stream);
                        }

                        // Salva a imagem no banco de dados associada à peça
                        _context.Imagens.Add(new Imagem
                        {
                            NomeArquivo = fileName,
                            PecaId = peca.IdPeca
                        });
                    }
                }
                
                await _context.SaveChangesAsync(); // Salva as imagens no banco
            }

            ViewBag.Sucesso = "Peça adicionada com sucesso!";
            return View();
        }

        //Formulário de edição da peça (GET)
        [HttpGet]
        public async Task<IActionResult> EditarPeca(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var peca = await _context.Pecas.FindAsync(id);
            if (peca == null) return NotFound();
            return View(peca);
        }

        //Editar moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPeca(Peca pecas)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(pecas);
            }

            var pecaExistente = await _context.Pecas.FindAsync(pecas.IdPeca);
            if (pecaExistente == null) return NotFound();

            // Atualiza os campos editáveis manualmente
            pecaExistente.Nome = pecas.Nome;
            pecaExistente.Descricao = pecas.Descricao;
            pecaExistente.Categoria = pecas.Categoria;
            pecaExistente.Marca = pecas.Marca;
            pecaExistente.Modelo = pecas.Modelo;
            pecaExistente.Compatibilidade = pecas.Compatibilidade;
            pecaExistente.Preco = pecas.Preco;
            pecaExistente.DataFabricacao = pecas.DataFabricacao;
            pecaExistente.Stock = pecas.Stock;
            pecaExistente.Estado = pecas.Estado;
            pecaExistente.Peso = pecas.Peso;
            pecaExistente.Garantia = pecas.Garantia;

            _context.Pecas.Update(pecaExistente);
            await _context.SaveChangesAsync();

            ViewBag.Sucesso = "Peça editada com sucesso!";
            return View(pecaExistente);
        }

        public async Task<IActionResult> ExcluirPeca(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var peca = await _context.Pecas
                .Include(m => m.AnuncioPecas) // Garante que os anúncios associados são carregados
                .FirstOrDefaultAsync(m => m.IdPeca == id);

            if (peca == null)
            {
                return NotFound();
            }

            // Remove a peça (os anúncios serão apagados automaticamente por `ON DELETE CASCADE`)
            _context.Pecas.Remove(peca);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "DashBoard");
        }
    }
}
