using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class ContactosController : Controller
    {
        private readonly AppDbContext _context;

        public ContactosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Contactos()
        {
            // Obter o ID do cliente logado através do Claim "IdCliente"
            var userId = User.FindFirstValue("IdCliente");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idCliente))
            {
                Console.WriteLine("Erro ao obter ID do cliente logado.");
                return Json(new { success = false, message = "Erro ao identificar o cliente logado." });
            }
            Console.WriteLine($"ID do Cliente Logado: {idCliente}");
            // O ID pode agora ser usado para qualquer operação.
            ViewBag.IdCliente = idCliente; // Passa para a View

            return View();
        }

        [HttpGet]
        public IActionResult OrcamentoEnviado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(OrcamentoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Retorna para a View Contactos e exibe os erros de validação
                return View("Contactos", model);
            }

            // Mapear os dados do ViewModel para a entidade Orcamento
            var orcamento = new Orcamento
            {
                IdCliente = model.IdCliente, // Associe a um cliente específico, se aplicável
                Descricao = model.Descricao,
                DataCriacao = DateTime.Now,
                ValorTotal = 0.0, // Valor definido posteriormente pelo administrador
                PrazoResposta = model.PrazoResposta,
                MetodoPagamento = model.MetodoPagamento,
                Status = "Pendente",
            };

            // Salvar no banco de dados
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrcamentoEnviado"); // Redireciona para uma página de sucesso
        }
    }
}
