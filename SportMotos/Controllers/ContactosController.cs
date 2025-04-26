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
        [HttpPost]
        public IActionResult AceitarOrcamento(int id)
        {
            var orcamento = _context.Orcamentos.FirstOrDefault(o => o.IdOrcamento == id);

            if (orcamento == null)
            {
                return NotFound();
            }

            if (!orcamento.DetalhesVisualizados)
            {
                return BadRequest("Você deve visualizar os detalhes do orçamento antes de aceitá-lo.");
            }

            orcamento.Status = "Aprovado";
            orcamento.UltimaAtualizacao = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        public IActionResult RejeitarOrcamento(int id)
        {
            var orcamento = _context.Orcamentos.FirstOrDefault(o => o.IdOrcamento == id);

            if (orcamento == null)
            {
                return Json(new { success = false, message = "Orçamento não encontrado." });
            }

            orcamento.Status = "Rejeitado";
            orcamento.UltimaAtualizacao = DateTime.Now;

            _context.SaveChanges();

            return Json(new { success = true, message = "Orçamento rejeitado com sucesso." });
        }

        [HttpGet]
        public IActionResult DetalhesOrcamento(int id)
        {
            Console.WriteLine($"Carregando detalhes do orçamento com Id: {id}");

            var orcamento = _context.Orcamentos
                .Include(o => o.IdClienteNavigation)
                .FirstOrDefault(o => o.IdOrcamento == id);

            if (orcamento == null)
            {
                Console.WriteLine("Orçamento não encontrado.");
                return NotFound();
            }

            Console.WriteLine($"Orçamento encontrado: Id={orcamento.IdOrcamento}, ClienteId={orcamento.IdCliente}, Status={orcamento.Status}");

            if (!orcamento.DetalhesVisualizados)
            {
                Console.WriteLine("Atualizando campo DetalhesVisualizados para true.");
                orcamento.DetalhesVisualizados = true;
                _context.SaveChanges();
            }

            var pecasDisponiveis = _context.Pecas
                .Where(p => p.Stock > 0)
                .ToList();

            Console.WriteLine($"Peças disponíveis carregadas: {pecasDisponiveis.Count}");

            var viewModel = new DetalhesOrcamentoViewModel
            {
                Orcamento = orcamento,
                PecasDisponiveis = pecasDisponiveis
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarPecasSelecionadas(int IdOrcamento, List<int> PecasSelecionadas, Dictionary<int, int> Quantidades)
        {
            Console.WriteLine($"IdOrcamento recebido: {IdOrcamento}");
            Console.WriteLine($"PecasSelecionadas recebidas: {string.Join(", ", PecasSelecionadas)}");

            foreach (var kvp in Quantidades)
            {
                Console.WriteLine($"Quantidade recebida para PecaId {kvp.Key}: {kvp.Value}");
            }

            var orcamento = _context.Orcamentos.FirstOrDefault(o => o.IdOrcamento == IdOrcamento);
            if (orcamento == null)
            {
                Console.WriteLine("Orçamento não encontrado.");
                return RedirectToAction("DetalhesOrcamento", new { id = IdOrcamento });
            }

            if (PecasSelecionadas == null || !PecasSelecionadas.Any())
            {
                Console.WriteLine("Nenhuma peça selecionada.");
                return RedirectToAction("DetalhesOrcamento", new { id = IdOrcamento });
            }

            foreach (var pecaId in PecasSelecionadas)
            {
                var quantidade = Quantidades.ContainsKey(pecaId) ? Quantidades[pecaId] : 0;
                Console.WriteLine($"Processando PecaId: {pecaId}, Quantidade: {quantidade}");

                if (quantidade > 0)
                {
                    var orcamentoPeca = new OrcamentoPeca
                    {
                        IdOrcamento = IdOrcamento,
                        IdPeca = pecaId,
                        Quantidade = quantidade
                    };

                    Console.WriteLine($"Adicionando OrcamentoPeca: IdOrcamento={orcamentoPeca.IdOrcamento}, IdPeca={orcamentoPeca.IdPeca}, Quantidade={orcamentoPeca.Quantidade}");
                    _context.OrcamentoPeca.Add(orcamentoPeca);

                    var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == pecaId);
                    if (peca != null)
                    {
                        Console.WriteLine($"Atualizando estoque da peça {pecaId}. Estoque atual: {peca.Stock}, Reduzindo: {quantidade}");
                        peca.Stock -= quantidade;
                    }
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Alterações salvas no banco de dados.");

            return RedirectToAction("DetalhesOrcamento", new { id = IdOrcamento });
        }
    }
}
