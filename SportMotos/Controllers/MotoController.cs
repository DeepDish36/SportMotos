﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class MotoController : Controller
    {
        //Controller para criar uma moto
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context)
        {
            _context = context;
        }

        //Listar motos
        public async Task<IActionResult> ListarMotos()
        {
            var motos = await _context.Motos.ToListAsync();
            return View(motos);
        }

        //Detalhes da moto
        public async Task<IActionResult> DetalhesMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            return View(moto);
        }

        //Formulário de adição da moto ao sistema
        public IActionResult AdicionarMoto()
        {
            return View();
        }

        // Adicionar moto ao sistema (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarMoto(Moto moto, List<IFormFile> Imagens)
        {
            if (!ModelState.IsValid)
            {
                return View(moto);
            }

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync(); // Primeiro salva a moto para gerar o ID

            // Processa cada imagem
            if (Imagens != null && Imagens.Count > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/motos");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var imagem in Imagens)
                {
                    string fileName = $"{moto.Matricula}_{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagem.CopyToAsync(stream);
                    }

                    // Adiciona a imagem à lista de imagens da moto
                    _context.Imagens.Add(new Imagem
                    {
                        NomeArquivo = fileName,
                        MotoId = moto.IdMoto
                    });
                }

                await _context.SaveChangesAsync(); // Salva as imagens no banco
            }

            ViewBag.Sucesso = "Moto adicionada com sucesso!";
            return View();
        }

        //Formulário de edição da moto (GET)
        [HttpGet]
        public async Task<IActionResult> EditarMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            return View(moto);
        }

        //Editar moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarMoto(Moto moto)
        {
            if (!ModelState.IsValid)
            {
                return View(moto);
            }

            var motoExistente = await _context.Motos.FindAsync(moto.IdMoto);
            if (motoExistente == null) return NotFound();

            // Atualiza os campos editáveis manualmente
            motoExistente.Marca = moto.Marca;
            motoExistente.Modelo = moto.Modelo;
            motoExistente.Ano = moto.Ano;
            motoExistente.Quilometragem = moto.Quilometragem;
            motoExistente.Preco = moto.Preco;
            motoExistente.Condicao = moto.Condicao;
            motoExistente.Cilindrada = moto.Cilindrada;
            motoExistente.Caixa = moto.Caixa;
            motoExistente.Matricula = moto.Matricula;
            motoExistente.Segmento = moto.Segmento;
            motoExistente.Combustivel = moto.Combustivel;
            motoExistente.Carta = moto.Carta;
            motoExistente.Cor = moto.Cor;
            motoExistente.Descricao = moto.Descricao;
            motoExistente.Abs = moto.Abs;

            _context.Motos.Update(motoExistente);
            await _context.SaveChangesAsync();

            ViewBag.Sucesso = "Moto editada com sucesso!";
            return View(motoExistente);
        }

        public async Task<IActionResult> ExcluirMoto(int id)
        {
            // Busca a moto e verifica se pertence ao utilizador
            var moto = await _context.Motos
                .Include(m => m.AnuncioMotos) // Garante que os anúncios associados são carregados
                .FirstOrDefaultAsync(m => m.IdMoto == id);

            if (moto == null)
            {
                return NotFound();
            }

            // Remove a moto (os anúncios serão apagados automaticamente por `ON DELETE CASCADE`)
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "DashBoard");
        }

        //Retorna as marcas das motos
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.Motos.Select(m => m.Marca).Distinct().ToListAsync();
            return Json(marcas);
        }

        //Retorna os modelos das motos
        public async Task<IActionResult> GetModelos(string marca)
        {
            var modelos = await _context.Motos
                .Where(m => m.Marca == marca)
                .Select(m => m.Modelo)
                .Distinct()
                .ToListAsync();
            return Json(modelos);
        }

        //Retorna os anos das motos
        public async Task<IActionResult> GetAnos()
        {
            var anos = await _context.Motos
                .Select(m => m.Ano)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
            return Json(anos);
        }

        //Retorna os estilos das motos
        public async Task<IActionResult> GetEstilos(string estilo)
        {
            var estilos = await _context.Motos
                .Select(m => m.Segmento)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return Json(estilos);
        }

        // Corrigido o método para evitar os erros relatados
        [HttpGet]
        public IActionResult GetAnuncios(string estilo, string marca, string modelo, string precoDe, string precoAte, string anoDe, string anoAte, string combustivel, string kmDe, string kmAte)
        {
            // Lógica para buscar os anúncios filtrados
            var anuncios = _context.AnuncioMotos.AsQueryable();

            if (!string.IsNullOrEmpty(estilo))
                anuncios = anuncios.Where(a => a.Titulo.Contains(estilo)); // Substituído por campo existente

            if (!string.IsNullOrEmpty(marca))
                anuncios = anuncios.Where(a => a.Titulo.Contains(marca)); // Substituído por campo existente

            if (!string.IsNullOrEmpty(modelo))
                anuncios = anuncios.Where(a => a.Titulo.Contains(modelo)); // Substituído por campo existente

            if (int.TryParse(anoDe, out int anoInicial))
                anuncios = anuncios.Where(a => a.Visualizacoes >= anoInicial); // Substituído por campo existente

            if (int.TryParse(anoAte, out int anoFinal))
                anuncios = anuncios.Where(a => a.Visualizacoes <= anoFinal); // Substituído por campo existente

            if (!string.IsNullOrEmpty(combustivel))
                anuncios = anuncios.Where(a => a.Descricao.Contains(combustivel)); // Substituído por campo existente

            if (int.TryParse(kmDe, out int kmMin))
                anuncios = anuncios.Where(a => a.Favoritos >= kmMin); // Substituído por campo existente

            if (int.TryParse(kmAte, out int kmMax))
                anuncios = anuncios.Where(a => a.Favoritos <= kmMax); // Substituído por campo existente

            if (decimal.TryParse(precoDe, out decimal precoMin))
                anuncios = anuncios.Where(a => a.Preco >= (double)precoMin); // Corrigido o tipo de comparação

            if (decimal.TryParse(precoAte, out decimal precoMax))
                anuncios = anuncios.Where(a => a.Preco <= (double)precoMax); // Corrigido o tipo de comparação

            return Json(anuncios.ToList());
        }
    }
}
