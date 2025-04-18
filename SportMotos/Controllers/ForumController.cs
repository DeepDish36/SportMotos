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
                .Include(f => f.IdAdminNavigation)
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
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Erro de validação: {error.ErrorMessage}");
                    }
                    return View(forum);
                }

                // 🔍 Verifica se o usuário está autenticado corretamente
                if (!User.Identity.IsAuthenticated)
                {
                    ModelState.AddModelError("", "Usuário não autenticado.");
                    Console.WriteLine("Erro: O usuário não está autenticado.");
                    return View(forum);
                }

                // Captura os claims do usuário autenticado
                var userIdClaim = User.FindFirst("IdCliente")?.Value; // ID do cliente autenticado
                var adminIdClaim = User.FindFirst("IdAdmin")?.Value; // ID do admin autenticado
                var userType = User.FindFirst("Tipo_Utilizador")?.Value; // Tipo de utilizador ("Cliente" ou "Admin")

                // Depuração: Mostra os claims disponíveis
                var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                Console.WriteLine($"Claims do usuário: {string.Join(", ", claims)}");

                if (string.IsNullOrEmpty(userIdClaim) && string.IsNullOrEmpty(adminIdClaim))
                {
                    ModelState.AddModelError("", "Usuário não autenticado ou sem permissões.");
                    Console.WriteLine("Erro: Usuário sem claims necessários.");
                    return View(forum);
                }

                // Configuração do fórum
                if (userType == "Cliente" && !string.IsNullOrEmpty(userIdClaim))
                {
                    forum.IdCliente = int.Parse(userIdClaim);
                    forum.IdAdmin = null;
                }
                else if (userType == "Admin" && !string.IsNullOrEmpty(adminIdClaim))
                {
                    forum.IdAdmin = int.Parse(adminIdClaim);
                    forum.IdCliente = null;
                }
                else
                {
                    ModelState.AddModelError("", "Tipo de usuário inválido.");
                    Console.WriteLine("Erro: Tipo de usuário inválido.");
                    return View(forum);
                }

                forum.DataCriacao = DateTime.Now;
                forum.Estado = "Ativo";
                forum.Categoria ??= "Outros";

                _context.Add(forum);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Fórum criado com sucesso. ID: {forum.IdForum}");
                return RedirectToAction("DetalhesForum", "Forum", new { id = forum.IdForum });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar o fórum: {ex.Message}");
                ModelState.AddModelError("", "Ocorreu um erro ao criar o fórum. Tente novamente.");
                return View(forum);
            }
        }

        public async Task<IActionResult> DetalhesForum(int id)
        {
            // Busca o fórum pelo ID, incluindo as respostas e informações do criador
            var forum = await _context.Forums
                .Include(f => f.Respostas)
                    .ThenInclude(r => r.IdClienteNavigation) // Inclui nome do cliente que respondeu
                .Include(f => f.IdClienteNavigation) // Inclui nome do cliente que criou
                .Include(f => f.IdAdminNavigation)   // Inclui nome do admin que criou
                .FirstOrDefaultAsync(f => f.IdForum == id);

            // Se não encontrar o fórum, retorna erro 404
            if (forum == null)
            {
                return NotFound();
            }

            return View(forum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarResposta(int IdForum, string Conteudo)
        {
            if (string.IsNullOrWhiteSpace(Conteudo))
            {
                ModelState.AddModelError("", "A resposta não pode estar vazia.");
                return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
            }

            var userIdClaim = User.FindFirst("IdCliente")?.Value;
            var adminIdClaim = User.FindFirst("IdAdmin")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) && string.IsNullOrEmpty(adminIdClaim))
            {
                ModelState.AddModelError("", "Utilizador não autenticado.");
                return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
            }

            // Define os valores corretos para Cliente e Admin
            int? idCliente = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : null;
            int? idAdmin = !string.IsNullOrEmpty(adminIdClaim) ? int.Parse(adminIdClaim) : null;

            // 🔥 Inserção manual via SQL para evitar conflito com triggers
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO Resposta (ID_Forum, Conteudo, Data_Criacao, ID_Cliente, ID_Admin) VALUES (@p0, @p1, @p2, @p3, @p4)",
                IdForum, Conteudo, DateTime.Now, idCliente, idAdmin);

            return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
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
