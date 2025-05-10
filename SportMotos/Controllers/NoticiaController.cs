using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class NoticiaController : Controller
    {
        private readonly AppDbContext _context;

        public NoticiaController(AppDbContext context)
        {
            _context = context;
        }

        // Exibir TODAS as notícias (Acesso para todos)
        public async Task<IActionResult> Noticias()
        {
            var noticias = await _context.Noticia.ToListAsync();

            if (!noticias.Any())
            {
                return NotFound("Nenhuma notícia encontrada.");
            }

            return View(noticias);
        }

        public async Task<IActionResult> DetalhesNoticia(int id)
        {
            var noticia = await _context.Noticia.FindAsync(id);
            if (noticia == null)
            {
                return NotFound("Notícia não encontrada.");
            }
            return View(noticia);
        }

        [HttpGet]
        public IActionResult AdicionarNoticia()
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarNoticia(Noticium noticia)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var sanitizer = new Ganss.Xss.HtmlSanitizer();
                noticia.Descricao = sanitizer.Sanitize(noticia.Descricao);
                noticia.DataPublicacao = DateTime.Now;
                noticia.DataEdicao = null;
                noticia.ApagadoEm = null;

                _context.Add(noticia);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Noticias));
            }
            return View(noticia);
        }

        public async Task<IActionResult> EditarNoticia(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

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
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != noticia.IdNoticia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var noticiaOriginal = await _context.Noticia.AsNoTracking().FirstOrDefaultAsync(n => n.IdNoticia == id);
                    if (noticiaOriginal == null)
                    {
                        return NotFound();
                    }

                    noticia.DataPublicacao = noticiaOriginal.DataPublicacao;

                    var sanitizer = new Ganss.Xss.HtmlSanitizer();
                    noticia.Descricao = string.IsNullOrWhiteSpace(noticia.Descricao)
                        ? noticiaOriginal.Descricao
                        : sanitizer.Sanitize(noticia.Descricao);

                    noticia.DataEdicao = DateTime.Now;

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

                ViewBag.Sucesso = "Notícia editada com sucesso!";
                return View(noticia);
            }

            return View(noticia);
        }

        public async Task<IActionResult> ExcluirNoticia(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var noticia = await _context.Noticia.FindAsync(id);
            if (noticia == null)
            {
                return NotFound("Notícia não encontrada.");
            }

            _context.Noticia.Remove(noticia);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "DashBoard");
        }
    }
}
