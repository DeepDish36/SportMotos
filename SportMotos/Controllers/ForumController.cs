using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
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
            Console.WriteLine("Método AdicionarResposta acionado!");

            // Verificar se o conteúdo é válido
            if (string.IsNullOrWhiteSpace(Conteudo))
            {
                Console.WriteLine("Erro: Conteúdo da resposta está vazio.");
                ModelState.AddModelError("", "A resposta não pode estar vazia.");
                return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
            }

            // Capturar IDs do Cliente e do Admin
            var userIdClaim = User.FindFirst("IdCliente")?.Value;
            var adminIdClaim = User.FindFirst("IdAdmin")?.Value;

            Console.WriteLine($"ID Cliente encontrado no Claim: {userIdClaim}");
            Console.WriteLine($"ID Admin encontrado no Claim: {adminIdClaim}");

            // Verificar autenticação
            if (string.IsNullOrEmpty(userIdClaim) && string.IsNullOrEmpty(adminIdClaim))
            {
                Console.WriteLine("Erro: Utilizador não autenticado.");
                ModelState.AddModelError("", "Utilizador não autenticado.");
                return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
            }

            // Definir ID correto para Cliente/Admin
            int? idCliente = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : null;
            int? idAdmin = !string.IsNullOrEmpty(adminIdClaim) ? int.Parse(adminIdClaim) : null;

            Console.WriteLine($"ID Cliente processado: {idCliente}");
            Console.WriteLine($"ID Admin processado: {idAdmin}");

            try
            {
                // 🔥 Inserção manual via SQL para evitar conflitos com triggers
                Console.WriteLine($"Tentando inserir resposta no Fórum: IDForum = {IdForum}, Conteúdo = {Conteudo}, Data = {DateTime.Now}, ID Cliente = {idCliente}, ID Admin = {idAdmin}");
                await _context.Database.ExecuteSqlRawAsync(
                     "INSERT INTO Resposta (ID_Forum, Conteudo, Data_Criacao, ID_Cliente, ID_Admin) VALUES (@p0, @p1, @p2, @p3, @p4)",
                     IdForum,
                     Conteudo,
                     DateTime.Now,
                     idCliente.HasValue ? idCliente.Value : null,
                     idAdmin.HasValue ? idAdmin.Value : null
                 );


                Console.WriteLine("Resposta inserida com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir resposta: {ex.Message}");
                ModelState.AddModelError("", "Erro ao inserir resposta no fórum.");
            }

            return RedirectToAction("DetalhesForum", "Forum", new { id = IdForum });
        }

        [Authorize]
        public async Task<IActionResult> EditarForum(int id)
        {
            var forum = await _context.Forums.FindAsync(id);
            if (forum == null) return NotFound();

            var idClienteLogado = User.FindFirst("IdCliente")?.Value;
            var idAdminLogado = User.FindFirst("IdAdmin")?.Value;

            // Permitir apenas ao autor ou ao administrador editar
            if (forum.IdCliente.ToString() != idClienteLogado && idAdminLogado == null)
            {
                return Unauthorized();
            }

            return View(forum);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarForum(Forum forum)
        {
            if (!ModelState.IsValid) return View(forum);

            var idClienteLogado = User.FindFirst("IdCliente")?.Value;
            var idAdminLogado = User.FindFirst("IdAdmin")?.Value;

            var forumExistente = await _context.Forums.FindAsync(forum.IdForum);
            if (forumExistente == null) return NotFound();

            // Verificar se o usuário tem permissão para editar
            if (forumExistente.IdCliente.ToString() != idClienteLogado && idAdminLogado == null)
            {
                return Unauthorized();
            }

            forumExistente.Titulo = forum.Titulo;
            forumExistente.Descricao = forum.Descricao;
            forumExistente.Categoria = forum.Categoria;
            forumExistente.DataEdicao = DateTime.Now;

            // Apenas um ADMIN pode alterar o estado do fórum
            if (!string.IsNullOrEmpty(idAdminLogado))
            {
                forumExistente.Estado = forum.Estado;
            }

            _context.Forums.Update(forumExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetalhesForum", new { id = forum.IdForum });
        }

        [Authorize]
        public async Task<IActionResult> ApagarForum(int id)
        {
            var forum = await _context.Forums.FindAsync(id);
            if (forum == null) return NotFound();

            var idClienteLogado = User.FindFirst("IdCliente")?.Value;
            var idAdminLogado = User.FindFirst("IdAdmin")?.Value;

            // Permitir exclusão apenas ao autor ou ao admin
            if (forum.IdCliente.ToString() != idClienteLogado && idAdminLogado == null)
            {
                return Unauthorized();
            }

            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction("Forum");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditarResposta(int id, [FromBody] Resposta respostaAtualizada)
        {
            Console.WriteLine($"🔍 Editando resposta ID: {id}");

            var resposta = await _context.Resposta
                .Include(r => r.IdClienteNavigation) // ✅ Carrega o cliente associado
                .FirstOrDefaultAsync(r => r.IdResposta == id);

            if (resposta == null)
            {
                Console.WriteLine("❌ Resposta não encontrada.");
                return NotFound("Resposta não encontrada.");
            }

            var idClienteLogado = User.FindFirst("IdCliente")?.Value;
            var idAdminLogado = User.FindFirst("IdAdmin")?.Value;

            Console.WriteLine($"🔍 Usuário logado: IdCliente={idClienteLogado}, IdAdmin={idAdminLogado}");
            Console.WriteLine($"🔍 Autor da resposta: IdCliente={resposta.IdClienteNavigation?.IdCliente}");

            if (resposta.IdClienteNavigation?.IdCliente.ToString() != idClienteLogado && string.IsNullOrEmpty(idAdminLogado))
            {
                Console.WriteLine("❌ Usuário sem permissão para editar.");
                return Unauthorized("Você não tem permissão para editar esta resposta.");
            }

            Console.WriteLine($"✅ Atualizando resposta: {respostaAtualizada.Conteudo}");

            try
            {
                // ⚡ Atualização manual no banco, evitando problemas com triggers
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Resposta SET Conteudo = {0} WHERE ID_Resposta = {1}", respostaAtualizada.Conteudo, id);

                Console.WriteLine("✅ Resposta editada com sucesso!");
                return Ok("Resposta editada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao salvar resposta: {ex.Message}");
                return StatusCode(500, "Erro ao salvar resposta no banco.");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> ApagarResposta(int id)
        {
            Console.WriteLine($"🔍 Tentando apagar resposta ID: {id}");

            var resposta = await _context.Resposta
                .Include(r => r.IdClienteNavigation) // ✅ Carrega o cliente associado
                .FirstOrDefaultAsync(r => r.IdResposta == id);

            if (resposta == null)
            {
                Console.WriteLine("❌ Resposta não encontrada.");
                return NotFound("Resposta não encontrada.");
            }

            var idClienteLogado = User.FindFirst("IdCliente")?.Value;
            var idAdminLogado = User.FindFirst("IdAdmin")?.Value;

            Console.WriteLine($"🔍 Usuário logado: IdCliente={idClienteLogado}, IdAdmin={idAdminLogado}");
            Console.WriteLine($"🔍 Autor da resposta: IdCliente={resposta.IdClienteNavigation?.IdCliente}");

            if (resposta.IdClienteNavigation?.IdCliente.ToString() != idClienteLogado && string.IsNullOrEmpty(idAdminLogado))
            {
                Console.WriteLine("❌ Usuário sem permissão para apagar.");
                return Unauthorized("Você não tem permissão para apagar esta resposta.");
            }

            try
            {
                // ⚡ Apaga a resposta do banco
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Resposta WHERE ID_Resposta = {0}", id);

                Console.WriteLine("✅ Resposta apagada com sucesso!");
                return Ok("Resposta excluída.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao apagar resposta: {ex.Message}");
                return StatusCode(500, "Erro ao apagar a resposta no banco.");
            }
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
