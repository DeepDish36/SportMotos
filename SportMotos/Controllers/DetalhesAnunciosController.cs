using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class DetalhesAnunciosController : Controller
    {
        private readonly AppDbContext _context;

        public DetalhesAnunciosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DetalheAnuncio(int id, string tipo)
        {
            if (id <= 0)
            {
                return NotFound(); // Se ID for inválido, retorna erro 404
            }

            if (tipo == "motos")
            {
                var anuncio = await _context.AnuncioMotos
                    .Include(a => a.IdMotoNavigation)
                    .ThenInclude(a => a.Imagens)
                    .FirstOrDefaultAsync(a => a.IdAnuncioMoto == id && a.ApagadoEm == null);

                ViewBag.TipoAnuncio = "motos";

                if (anuncio == null)
                {
                    return NotFound(); // Retorna erro 404 se não encontrar
                }

                anuncio.Visualizacoes++;
                _context.SaveChanges();

                return View(anuncio);
            }
            else if (tipo == "pecas")
            {
                var anuncio = await _context.AnuncioPecas
                    .Include(a => a.IdPecaNavigation)
                    .FirstOrDefaultAsync(a => a.IdAnuncioPeca == id && a.ApagadoEm == null);

                ViewBag.TipoAnuncio = "pecas";

                if (anuncio == null)
                {
                    return NotFound();
                }

                anuncio.Visualizacoes++;
                _context.SaveChanges();

                return View(anuncio);
            }

            return BadRequest("Anúncio Inválido"); // Redireciona para uma página de erro personalizada
        }
    }
}
