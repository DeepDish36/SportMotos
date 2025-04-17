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
                // Instanciar o HtmlSanitizer
                var sanitizer = new Ganss.Xss.HtmlSanitizer();

                // Sanitizar o campo Descricao
                noticia.Descricao = sanitizer.Sanitize(noticia.Descricao);

                // Inicializar campos de data
                noticia.DataPublicacao = DateTime.Now; // Sempre inicializado
                noticia.DataEdicao = null; // Pode ser nulo
                noticia.ApagadoEm = null; // Pode ser nulo

                // Salvar no banco de dados
                _context.Add(noticia);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Noticias));
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
                    // Buscar a notícia original para manter a Data_Publicacao
                    var noticiaOriginal = await _context.Noticia.AsNoTracking().FirstOrDefaultAsync(n => n.IdNoticia == id);
                    if (noticiaOriginal == null)
                    {
                        return NotFound();
                    }

                    // Mantém a data original de publicação
                    noticia.DataPublicacao = noticiaOriginal.DataPublicacao;

                    // Sanitizar a Descrição
                    var sanitizer = new Ganss.Xss.HtmlSanitizer();
                    noticia.Descricao = string.IsNullOrWhiteSpace(noticia.Descricao)
                        ? noticiaOriginal.Descricao
                        : sanitizer.Sanitize(noticia.Descricao);

                    // Atualizar a data de edição
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
            // Busca a notícia pelo ID
            var noticia = await _context.Noticia.FindAsync(id);

            if (noticia == null)
            {
                return NotFound("Notícia não encontrada.");
            }

            // Remove a notícia
            _context.Noticia.Remove(noticia);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "DashBoard");
        }
    }
}
