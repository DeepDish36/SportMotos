using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using SportMotos.Helpers;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using SportMotos.Services;
using System.Text;

namespace SportMotos.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public CarrinhoController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [Authorize]
        public IActionResult AdicionarAoCarrinho(int idPeca)
        {
            // 🔥 Buscar o ID do cliente a partir das claims
            var idClienteClaim = User.Claims.FirstOrDefault(c => c.Type == "IdCliente");
            if (idClienteClaim == null) return Unauthorized(); // Verifica se a claim existe

            int idCliente = int.Parse(idClienteClaim.Value); // Converte o valor da claim para int

            // 🔥 Buscar a peça no banco de dados
            var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == idPeca);

            if (peca == null) return NotFound();

            var itemExistente = _context.CarrinhoCompras.FirstOrDefault(i => i.IdCliente == idCliente && i.IdPeca == idPeca);

            if (itemExistente != null)
            {
                // Aumentar a quantidade
                itemExistente.Quantidade++;
            }
            else
            {
                // Criar um novo registo na BD
                var novoItem = new CarrinhoCompras
                {
                    IdCliente = idCliente,
                    IdPeca = peca.IdPeca,
                    Quantidade = 1,
                    DataAdicionado = DateTime.Now
                };
                _context.CarrinhoCompras.Add(novoItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Cesta", new { idCliente });
        }

        [HttpGet]
        public JsonResult ObterIdCliente()
        {
            var idClienteClaim = User.Claims.FirstOrDefault(c => c.Type == "IdCliente");
            if (idClienteClaim == null)
            {
                return Json(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            int idCliente = int.Parse(idClienteClaim.Value);
            return Json(new { sucesso = true, idCliente });
        }

        public IActionResult DetalhesPedido(int id)
        {
            var itensPedido = _context.ItensPedido
                .Include(i => i.Peca) // ✅ Inclui informações das peças
                .Include(i => i.Pedido)
                    .ThenInclude(p => p.Cliente) // ✅ Inclui informações do cliente
                .Where(i => i.IdPedido == id)
                .ToList();

            if (!itensPedido.Any()) return NotFound();

            return View(itensPedido);
        }

        public JsonResult ObterCarrinho(int idCliente)
        {
            var total= _context.CarrinhoCompras
                .Where(i => i.IdCliente == idCliente)
                .Sum(i => i.Quantidade * i.Peca.Preco); // Calcula o total do carrinho
            var carrinho = _context.CarrinhoCompras
                .Where(i => i.IdCliente == idCliente)
                .Select(i => new
                {
                    id = i.IdPeca,
                    name = i.Peca.Nome,
                    brand = i.Peca.Marca,
                    price = i.Peca.Preco,
                    quantity = i.Quantidade,
                    image = "/images/pecas/" + i.IdPeca + ".jpg",
                    total
                })
                .ToList();

            var quantidadeTotal = carrinho.Sum(i => i.quantity); // Calcula a quantidade total

            return Json(new { carrinho, quantidadeTotal });
        }

        public IActionResult Cesta()
        {
            // 🔥 Obter o ID do cliente das Claims
            var userIdClaim = User.FindFirst("IdCliente")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            int idCliente = int.Parse(userIdClaim);

            // 🔥 Buscar os itens do carrinho no banco de dados
            var carrinho = _context.CarrinhoCompras
                .Include(c => c.Peca)
                .Where(i => i.IdCliente == idCliente)
                .ToList();

            return View(carrinho);
        }

        [HttpDelete]
        public JsonResult RemoverItem(int idPeca)
        {
            var userIdClaim = User.FindFirst("IdCliente")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            int idCliente = int.Parse(userIdClaim);

            var item = _context.CarrinhoCompras.FirstOrDefault(i => i.IdCliente == idCliente && i.IdPeca == idPeca);

            if (item == null)
            {
                return Json(new { sucesso = false, mensagem = "Item não encontrado no carrinho." });
            }

            _context.CarrinhoCompras.Remove(item);
            _context.SaveChanges();

            return Json(new { sucesso = true });
        }

        [HttpPost]
        public JsonResult AtualizarQuantidade([FromBody] AtualizarQuantidadeViewModel model)
        {
            var userIdClaim = User.FindFirst("IdCliente")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            int idCliente = int.Parse(userIdClaim);

            var item = _context.CarrinhoCompras.FirstOrDefault(i => i.IdCliente == idCliente && i.IdPeca == model.IdPeca);

            if (item == null)
            {
                return Json(new { sucesso = false, mensagem = "Item não encontrado no carrinho." });
            }

            if (model.Acao == "Aumentar")
            {
                item.Quantidade += 1;
            }
            else if (model.Acao == "Diminuir" && item.Quantidade > 1)
            {
                item.Quantidade -= 1;
            }

            _context.SaveChanges();

            return Json(new { sucesso = true, novaQuantidade = item.Quantidade });
        }

        public IActionResult Checkout()
        {
            var userIdClaim = User.FindFirst("IdCliente")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            int idCliente = int.Parse(userIdClaim);

            Console.WriteLine($"ID do cliente: {userIdClaim}");

            var carrinho = _context.CarrinhoCompras
                .Include(c => c.Peca)
                .Where(i => i.IdCliente == idCliente)
                .ToList();

            if (!carrinho.Any()) return RedirectToAction("Cesta");

            var model = new EnderecosEnvio { IdCliente = idCliente }; // 🔥 Passando ID no modelo

            ViewBag.Carrinho = carrinho;
            ViewBag.TotalCompra = carrinho.Sum(item => item.Quantidade * item.Peca.Preco);

            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessarCheckout(EnderecosEnvio model)
        {
            var userIdClaim = User.FindFirst("IdCliente")?.Value;
            Console.WriteLine($"IdCliente: {userIdClaim}");

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int idCliente))
            {
                return RedirectToAction("Login", "Login"); // Redireciona para login se não estiver autenticado
            }

            // Verifica se há itens no carrinho antes de criar o pedido
            var carrinho = _context.CarrinhoCompras
                .Include(c => c.Peca)
                .Where(c => c.IdCliente == idCliente)
                .ToList();

            if (!carrinho.Any())
            {
                TempData["Erro"] = "O carrinho está vazio! Adicione itens antes de finalizar a compra.";
                return RedirectToAction("Checkout");
            }

            // ✅ Criar o pedido corretamente com LINQ
            var pedido = new Pedidos
            {
                IdCliente = idCliente,
                DataCompra = DateTime.Now,
                Status = "Pendente",
                Total = 0 // Inicializamos com 0 e atualizamos depois
            };

            _context.Pedidos.Add(pedido);
            _context.SaveChanges(); // ✅ Agora `pedido.IdPedido` já está disponível

            // ✅ Adicionar itens ao pedido com ID correto
            decimal totalPedido = 0;

            foreach (var item in carrinho)
            {
                var itemPedido = new ItensPedido
                {
                    IdPedido = pedido.IdPedido,
                    IdPeca = item.IdPeca,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = (decimal)item.Peca.Preco
                };

                _context.ItensPedido.Add(itemPedido);
                totalPedido += item.Quantidade * (decimal)item.Peca.Preco;

                // ✅ Decrementar estoque corretamente
                var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == item.IdPeca);
                if (peca != null)
                {
                    peca.Stock -= item.Quantidade;
                }
            }

            _context.SaveChanges(); // ✅ Salvar os itens e a atualização do estoque

            // ✅ Atualizar total do pedido corretamente
            pedido.Total = Math.Round(totalPedido, 2);
            _context.SaveChanges();

            // ✅ Limpar carrinho corretamente
            var carrinhoCliente = _context.CarrinhoCompras.Where(c => c.IdCliente == idCliente);
            _context.CarrinhoCompras.RemoveRange(carrinhoCliente);
            if (carrinhoCliente.Any()) // ✅ Só executa se houver itens no carrinho
            {
                _context.CarrinhoCompras.RemoveRange(carrinhoCliente);
                _context.SaveChanges();
            }

            // ✅ Enviar e-mail com a fatura
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == idCliente);
            if (cliente != null)
            {
                var email = cliente.Email;
                var assunto = "Fatura da sua compra - SportMotos";
                var mensagem = GerarFaturaEmail(carrinho, totalPedido, pedido.IdPedido);

                _emailService.SendEmailAsync(email, assunto, mensagem).Wait();
            }

            TempData["Sucesso"] = "Pedido concluído com sucesso!";
            return RedirectToAction("ResumoPedido", new { idPedido = pedido.IdPedido });
        }

        // Método auxiliar para gerar o conteúdo da fatura
        private string GerarFaturaEmail(List<CarrinhoCompras> carrinho, decimal totalPedido, int idPedido)
        {
            string mensagemHtml = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <h2 style='color: #007bff;'>Fatura - Pedido #{idPedido}</h2>
        <p>Obrigado por comprar na <strong>SportMotos</strong>! Aqui estão os detalhes da sua compra:</p>

        <table style='width: 100%; border-collapse: collapse; border: 1px solid #ddd;'>
            <thead>
                <tr style='background-color: #f8f9fa;'>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Peça</th>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Marca</th>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Modelo</th>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Quantidade</th>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Preço Unitário</th>
                    <th style='padding: 10px; border: 1px solid #ddd;'>Total</th>
                </tr>
            </thead>
            <tbody>";

            foreach (var item in carrinho)
            {
                mensagemHtml += $@"
                <tr>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{item.Peca.Nome}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{item.Peca.Marca}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{item.Peca.Modelo}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{item.Quantidade}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>€{item.Peca.Preco:F2}</td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>€{(item.Quantidade * item.Peca.Preco):F2}</td>
                </tr>";
            }

            mensagemHtml += $@"
            </tbody>
        </table>

        <h3 style='color: #28a745;'>Total: €{totalPedido:F2}</h3>
        <p>Se tiver dúvidas, entre em contato:</p>
        <p><strong>Tlm:</strong> 922333444</p>
        <p><strong>Tlf:</strong> 232876554</p>
        <p><strong>Email:</strong> <a href='mailto:sportmotos@gmail.com'>sportmotos@gmail.com</a></p>
    </body>
    </html>";

            return mensagemHtml;
        }

        public IActionResult ResumoPedido(int idPedido)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Peca)
                .FirstOrDefault(p => p.IdPedido == idPedido);

            if (pedido == null)
            {
                TempData["MensagemErro"] = "Pedido não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            return View(pedido);
        }

        public IActionResult PedidosCliente(int idCliente)
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Peca)
                .Where(p => p.IdCliente == idCliente)
                .OrderByDescending(p => p.DataCompra)
                .ToList();

            if (!pedidos.Any()) TempData["MensagemErro"] = "Nenhum pedido encontrado.";

            return View(pedidos);
        }

        public IActionResult FinalizarCompra(int idCliente)
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<CarrinhoCompras>>("Carrinho") ?? new List<CarrinhoCompras>();
            if (!carrinho.Any()) return RedirectToAction("Cesta");

            foreach (var item in carrinho)
            {
                var peca = _context.Pecas.Find(item.IdPeca);
                if (peca == null || item.Quantidade > peca.Stock)
                {
                    TempData["Erro"] = $"A peça {peca?.Nome ?? "desconhecida"} não tem estoque suficiente!";
                    return RedirectToAction("Cesta");
                }
            }

            var novoPedido = new Pedidos
            {
                IdCliente = idCliente,
                DataCompra = DateTime.Now,
                Total = (decimal)carrinho.Sum(item => item.Quantidade * (double)_context.Pecas.Find(item.IdPeca).Preco),
                Status = "Pendente"
            };

            _context.Pedidos.Add(novoPedido);
            _context.SaveChanges();

            foreach (var item in carrinho)
            {
                var peca = _context.Pecas.Find(item.IdPeca);

                _context.ItensPedido.Add(new ItensPedido
                {
                    IdPedido = novoPedido.IdPedido,
                    IdPeca = item.IdPeca,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = (decimal)peca.Preco
                });

                var anuncio = _context.AnuncioPecas.FirstOrDefault(a => a.IdAnuncioPeca == peca.IdPeca);

                _context.VendaPeca.Add(new VendaPeca
                {
                    IdAnuncio = anuncio?.IdAnuncioPeca ?? 0,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = (decimal)peca.Preco,
                    DataVenda = DateTime.Now
                });

                peca.Stock -= item.Quantidade;
                if (peca.Stock <= 0 && anuncio != null)
                {
                    anuncio.Vendido = true;
                }
            }

            _context.SaveChanges();

            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("ResumoPedido", new { idPedido = novoPedido.IdPedido });
        }
    }
}
