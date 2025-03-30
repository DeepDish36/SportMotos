using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportMotos.Controllers
{
    [Authorize]
    public class CriarAnuncioController : Controller
    {
        private readonly AppDbContext _context;

        public CriarAnuncioController(AppDbContext context)
        {
            _context = context;
        }

        // Listar anúncios
        [HttpGet]
        public async Task<IActionResult> ShowAnuncios()
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anunciosMotos = await _context.AnuncioMotos.Include(a => a.IdMotoNavigation).ToListAsync();
            var anunciosPecas = await _context.AnuncioPecas.Include(a => a.IdPecaNavigation).ToListAsync();

            ViewBag.AnunciosMotos = anunciosMotos;
            ViewBag.AnunciosPecas = anunciosPecas;
            return View();
        }

        // *--------------------CRUD ANÚNCIOS--------------------* //

        // Criar anúncio de moto
        [HttpGet]
        public IActionResult CriarAnuncioMoto()
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar motos que não estão associadas a um anúncio
            var motosDisponiveis = _context.Motos
                .Where(m => !_context.AnuncioMotos.Any(a => a.IdMoto == m.IdMoto))
                .Select(m => new { m.IdMoto, m.Marca, m.Modelo })
                .ToList();

            // Preencher a ViewBag com as motos disponíveis
            ViewBag.MotosDisponiveis = new SelectList(motosDisponiveis, "IdMoto", "Marca");

            ViewBag.Motos = _context.Motos.ToList(); // Pega todas as motos cadastradas
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarAnuncioMoto(AnuncioMoto anuncio)
        {
            if (!ModelState.IsValid)
            {
                // Captura os erros do ModelState e exibe na ViewBag
                ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
                return View(anuncio);
            }

            anuncio.DataPublicacao = DateTime.Now;
            anuncio.Visualizacoes = 0;
            anuncio.Favoritos = 0;
            anuncio.Avaliacoes = 0;
            _context.AnuncioMotos.Add(anuncio);
            await _context.SaveChangesAsync();

            // Define uma mensagem de sucesso na ViewBag
            ViewBag.Success = "Anúncio publicado com sucesso!";

            return View(anuncio); // Mantém o usuário na mesma página
        }

        // Criar anúncio de peça
        [HttpGet]
        public IActionResult CriarAnuncioPeca()
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar peças que não estão associadas a um anúncio
            var pecasDisponiveis = _context.Pecas
                .Where(m => !_context.AnuncioPecas.Any(a => a.IdPeca == m.IdPeca))
                .Select(m => new { m.IdPeca, m.Nome, m.Categoria, m.Modelo })
                .ToList();

            // Preencher a ViewBag com as peças disponíveis
            ViewBag.PecasDisponiveis = new SelectList(pecasDisponiveis, "IdPeca", "Nome");

            ViewBag.Pecas = _context.Pecas.ToList(); // Vai buscar todas as peças cadastradas
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> CriarAnuncioPeca(AnuncioPeca anuncio)
        {
            if (!ModelState.IsValid)
            {
                // Captura os erros do ModelState e exibe na ViewBag
                ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
                return View(anuncio);
            }

            anuncio.DataPublicacao = DateTime.Now;
            anuncio.Visualizacoes = 0;
            anuncio.Favoritos = 0;
            anuncio.Avaliacoes = 0;
            _context.AnuncioPecas.Add(anuncio);
            await _context.SaveChangesAsync();

            // Define uma mensagem de sucesso na ViewBag
            ViewBag.Success = "Anúncio publicado com sucesso!";

            return View(anuncio); // Mantém o usuário na mesma página
        }

        // Editar anúncio
        public async Task<IActionResult> EditarAnuncioMoto(int id)
        {
            var anuncio = await _context.AnuncioMotos.FindAsync(id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        [HttpPost]
        public async Task<IActionResult> EditarAnuncioMoto(AnuncioMoto anuncio)
        {
            if (ModelState.IsValid)
            {
                anuncio.DataEdicao = DateTime.Now;
                _context.Update(anuncio);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(anuncio);
        }

        // Marcar como vendido
        public async Task<IActionResult> MarcarComoVendido(int id, string tipo)
        {
            if (tipo == "Moto")
            {
                var anuncio = await _context.AnuncioMotos.FindAsync(id);
                if (anuncio != null)
                {
                    anuncio.Vendido = true;
                    await _context.SaveChangesAsync();
                }
            }
            else if (tipo == "Peca")
            {
                var anuncio = await _context.AnuncioPecas.FindAsync(id);
                if (anuncio != null)
                {
                    anuncio.Vendido = true;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // Deletar anúncio
        public async Task<IActionResult> DeletarAnuncioMoto(int id)
        {
            var anuncio = await _context.AnuncioMotos.FindAsync(id);
            if (anuncio != null)
            {
                _context.AnuncioMotos.Remove(anuncio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeletarAnuncioPeca(int id)
        {
            var anuncio = await _context.AnuncioPecas.FindAsync(id);
            if (anuncio != null)
            {
                _context.AnuncioPecas.Remove(anuncio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // *-----------------------------------------------------* //

        // *--------------------DETALHES--------------------* //
        public IActionResult GetMotoDetails(int id)
        {
            var moto=_context.Motos.Find(id);

            if(moto==null)
            {
                return NotFound();
            }

            var motoDetails = new
            {
                marca = moto.Marca,
                modelo = moto.Modelo,
                preco = moto.Preco,
                condicao = moto.Condicao
            };

            return Json(motoDetails);
        }

        public IActionResult GetPecaDetails(int id)
        {
            var peca = _context.Pecas.Find(id);
            if (peca == null)
            {
                return NotFound();
            }
            var pecaDetails = new
            {
                nome = peca.Nome,
                marca = peca.Marca,
                modelo = peca.Modelo,
                categoria = peca.Categoria,
                preco = peca.Preco,
                condicao = peca.Estado
            };
            return Json(pecaDetails);
        }

        public IActionResult GetAnuncioMotoDetails(int id)
        {
            var anuncio = _context.AnuncioMotos
                .Include(a => a.IdMotoNavigation)
                .FirstOrDefault(a => a.IdMoto == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            var anuncioDetails = new
            {
                titulo = anuncio.Titulo,
                descricao = anuncio.Descricao,
                preco = anuncio.IdMotoNavigation?.Preco,
                marca = anuncio.IdMotoNavigation?.Marca,
                modelo = anuncio.IdMotoNavigation?.Modelo,
                condicao = anuncio.IdMotoNavigation?.Condicao
            };

            return Json(anuncioDetails);
        }
        // *-----------------------------------------------------* //
    }
}
