using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SportMotos.Controllers
{
    public class NoticiaController : Controller
    {
        private readonly AppDbContext _context;

        public NoticiaController(AppDbContext context)
        {
            _context = context;
        }

        // Exibir TODAS as notícias
        public async Task<IActionResult> Noticias()
        {
            var noticias = await _context.Noticia.ToListAsync(); // Buscar todas as notícias

            if (!noticias.Any()) // Se não houver notícias
            {
                return NotFound("Nenhuma notícia encontrada.");
            }

            return View(noticias); // Passa a lista de notícias para a View
        }
    }
}
