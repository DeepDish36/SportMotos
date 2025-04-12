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

        public async Task<IActionResult> DetalhesNoticia(int id)
        {
            var noticia = await _context.Noticia.FindAsync(id); // Buscar a notícia pelo ID
            if (noticia == null) // Se não encontrar a notícia
            {
                return NotFound("Notícia não encontrada.");
            }
            return View(noticia); // Passa a notícia para a View
        }

        [HttpGet]
        public IActionResult AdicionarNoticia()
        {
            return View(); // Retorna a View para adicionar uma nova notícia
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarNoticia(Noticium noticia)
        {
            if (ModelState.IsValid)
            {
                noticia.DataPublicacao = DateTime.Now;
                _context.Add(noticia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(noticia);
        }

        // Editar notícia
        public async Task<IActionResult> EditarNoticia(int id)
        {
            var noticia = await _context.Noticia.FindAsync(id);
            if (noticia == null)
            {
                return NotFound();
            }
            return View(noticia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarNoticia(int id, Noticium noticia)
        {
            if (id != noticia.IdNoticia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(noticia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Noticia.Any(e => e.IdNoticia == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(noticia);
        }

        // Excluir notícia
        public async Task<IActionResult> Delete(int id)
        {
            var noticia = await _context.Noticia.FindAsync(id);
            if (noticia == null)
            {
                return NotFound();
            }
            return View(noticia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noticia = await _context.Noticia.FindAsync(id);
            _context.Noticia.Remove(noticia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
