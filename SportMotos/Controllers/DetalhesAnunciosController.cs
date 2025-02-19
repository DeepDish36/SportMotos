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
            if (tipo == "motos")
            {
                var anuncio = await _context.AnuncioMotos
                .Include(a=>a.IdMotoNavigation)
                .ThenInclude(a=>a.Imagens)
                .Where(a => a.IdMoto == id && a.ApagadoEm == null)
                .FirstOrDefaultAsync(a=>a.IdAnuncioMoto==id);

                ViewBag.TipoAnuncio = "motos";

                if (anuncio == null)
                {
                    return NotFound();
                }

                return View(anuncio);
            }
            else if (tipo == "pecas")
            {
                var anuncio = await _context.AnuncioPecas
                .Include(a => a.IdPecaNavigation)
                .ThenInclude(a=>a.NomeArquivo)
                .Where(a => a.IdPeca == id && a.ApagadoEm == null)
                .FirstOrDefaultAsync();

                ViewBag.TipoAnuncio = "pecas";

                if (anuncio == null)
                {
                    return NotFound();
                }

                return View(anuncio);
            }

            return BadRequest("Anúncio Inválido");
        }
    }
}
