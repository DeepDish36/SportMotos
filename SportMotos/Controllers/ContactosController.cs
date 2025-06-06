﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using SportMotos.Services;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class ContactosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ContactosController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService= emailService;
        }

        [HttpGet]
        public IActionResult Contactos()
        {
            // Obter o ID do cliente logado através do Claim "IdCliente"
            var userId = User.FindFirstValue("IdCliente");
            Console.WriteLine($"ID do Cliente Logado: {userId}");
            // O ID pode agora ser usado para qualquer operação.
            ViewBag.IdCliente = userId; // Passa para a View

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
            var userIdClaim = User.FindFirstValue("IdCliente");

            // Verifica se o utilizador está autenticado
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            model.IdCliente = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return View("Contactos", model);
            }

            var orcamento = new Orcamento
            {
                IdCliente = model.IdCliente,
                Descricao = model.Descricao,
                DataCriacao = DateTime.Now,
                ValorTotal = 0.0,
                PrazoResposta = model.PrazoResposta,
                MetodoPagamento = model.MetodoPagamento,
                Status = "Pendente",
            };

            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrcamentoEnviado");
        }

        [HttpPost]
        public IActionResult AceitarOrcamento(int id)
        {
            var orcamento = _context.Orcamentos
                .Include(o => o.IdClienteNavigation)
                .Include(o => o.OrcamentoPecas)
                .ThenInclude(op => op.IdPecaNavigation)
                .FirstOrDefault(o => o.IdOrcamento == id);

            if (orcamento == null)
            {
                return NotFound();
            }

            if (!orcamento.DetalhesVisualizados)
            {
                return BadRequest("Você deve visualizar os detalhes do orçamento antes de aceitá-lo.");
            }

            // Calcular preço total
            double totalPreco = orcamento.OrcamentoPecas.Sum(op => op.IdPecaNavigation.Preco * op.Quantidade);

            // Montar a lista de peças utilizadas
            string listaPecasHtml = "<ul>";
            foreach (var op in orcamento.OrcamentoPecas)
            {
                listaPecasHtml += $"<li>{op.IdPecaNavigation.Nome} - {op.Quantidade} unidades - €{op.IdPecaNavigation.Preco * op.Quantidade}</li>";
            }
            listaPecasHtml += "</ul>";

            // Construir email
            string mensagemHtml = $@"
            <h2>Orçamento Aprovado</h2>
            <p>Prezado {orcamento.IdClienteNavigation.Nome},</p>
            <p>O seu orçamento foi aprovado! Veja os detalhes abaixo:</p>
    
            <h3>Descrição do problema:</h3>
            <p>{orcamento.Descricao}</p>
    
            <h3>Peças utilizadas:</h3>
            {listaPecasHtml}
    
            <p><strong>Preço total:</strong> €{totalPreco}</p>
    
            <hr>
            <p>Se tiver dúvidas, entre em contato:</p>
            <p><strong>Tlm:</strong> 922333444</p>
            <p><strong>Tlf:</strong> 232876554</p>
            <p><strong>Email:</strong> sportmotos@gmail.com</p>
        ";


            // Enviar email usando o serviço de email injetado
            _emailService.SendEmailAsync(orcamento.IdClienteNavigation.Email, "Orçamento Aprovado", mensagemHtml).Wait();

            // Atualizar status
            orcamento.Status = "Aprovado";
            orcamento.UltimaAtualizacao = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Dashboard","DashBoard");
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
