using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

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
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(forum);
            }

            var userIdClaim = User.FindFirst("IdCliente")?.Value; // ID do cliente autenticado
            var isAdmin = User.FindFirst("Tipo_Utilizador")?.Value == "Admin"; // Verifica se é admin

            if (string.IsNullOrEmpty(userIdClaim) && !isAdmin)
            {
                ModelState.AddModelError("", "Usuário não autenticado.");
                return View(forum);
            }

            Cliente? cliente = null;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente.ToString() == userIdClaim);
                if (cliente == null)
                {
                    ModelState.AddModelError("", "Usuário não encontrado na base de dados.");
                    return View(forum);
                }
            }

            // Configuração do fórum
            if (!isAdmin && cliente == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado na base de dados.");
                return View(forum);
            }

            // 🔥 Define `IdCliente` apenas para clientes, admins usam ID especial (ex: 0 ou um campo separado)
            forum.IdCliente = cliente?.IdCliente ?? 0;

            forum.DataCriacao = DateTime.Now;
            forum.Estado = "Ativo";
            forum.Categoria ??= "Outros";

            _context.Add(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetalhesForum", "Forum", new { id = forum.IdForum });
        }

        // Método para processar Markdown (podes ativar/desativar)
        private string ProcessarMarkdown(string descricao)
        {
            if (string.IsNullOrEmpty(descricao)) return "";

            // Aqui poderias implementar um parser Markdown, ex: Markdig
            return descricao;
        }
    }
}
