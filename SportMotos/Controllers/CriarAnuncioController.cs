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
            ViewBag.Pecas = _context.Pecas.ToList(); // Pega todas as peças cadastradas
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarAnuncioPeca(AnuncioPeca anuncio)
        {
            if (ModelState.IsValid)
            {
                anuncio.DataPublicacao = DateTime.Now;
                _context.AnuncioPecas.Add(anuncio);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(anuncio);
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
    }
}
