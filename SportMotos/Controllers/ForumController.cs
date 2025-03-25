using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class ForumController : Controller
    {
        private readonly AppDbContext _context;

        public ForumController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Forum()
        {
            var forums = await _context.Forums
                .Include(f => f.Respostas)
                .Include(f => f.IdClienteNavigation)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();

            return View(forums);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Forum forum)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View(forum);
            }

            // Supondo que o ID do cliente esteja armazenado na sessão ou User.Identity.Name
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);
            if (cliente == null)
            {
                ModelState.AddModelError("", "Erro ao identificar o cliente.");
                return View(forum);
            }

            forum.IdCliente = cliente.IdCliente;
            forum.DataCriacao = DateTime.Now;

            _context.Add(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
