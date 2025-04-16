using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class AnuncioController : Controller
    {
        private readonly AppDbContext _context;

        public AnuncioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Anuncio(string tipo, int page = 1, int pageSize = 12)
        {
            var tipoUtilizador = User.FindFirst("Tipo_Utilizador")?.Value;
            ViewBag.TipoUtilizador = tipoUtilizador;
            ViewBag.TipoAnuncio = tipo;

            if (tipo == "motos")
            {
                var query = _context.AnuncioMotos
                    .Where(a => a.ApagadoEm == null); // Apenas anúncios ativos

                int totalAnuncios = await query.CountAsync();
                var anunciosMotos = await query
                    .OrderByDescending(a => a.DataPublicacao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalAnuncios / pageSize);

                return View(anunciosMotos);
            }
            else if (tipo == "pecas")
            {
                var query = _context.AnuncioPecas
                    .Where(a => a.ApagadoEm == null)
                    .Include(a => a.IdPecaNavigation);

                int totalAnuncios = await query.CountAsync();
                var anunciosPecas = await query
                    .OrderByDescending(a => a.DataPublicacao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalAnuncios / pageSize);

                return View(anunciosPecas);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RenovarAnuncio(int id)
        {
            var anuncio = await _context.AnuncioMotos.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }

            anuncio.DataExpiracao = DateTime.Now.AddDays(50); // Renova por mais 30 dias
            _context.Update(anuncio);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}