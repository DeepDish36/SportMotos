using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using SportMotos.Helpers;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SportMotos.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;

        public CarrinhoController(AppDbContext context)
        {
            _context = context;
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

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Login"); // Redireciona para login se não estiver autenticado
            }

            model.IdCliente = int.Parse(userIdClaim); // Atribui corretamente o ID do cliente

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"❌ Erros de validação: {string.Join(", ", erros)}");

                TempData["Erro"] = "Preencha todos os campos corretamente!";
                return RedirectToAction("Checkout");
            }

            // Passo 1: Guardar o endereço de envio
            _context.EnderecosEnvios.Add(model);
            _context.SaveChanges();

            // Passo 2: Criar o pedido e associar ao cliente
            var pedido = new Pedidos
            {
                IdCliente = model.IdCliente,
                DataCompra = DateTime.Now,
                Status = "Pendente",
                Total = 0 // Será atualizado depois de calcular os itens
            };

            _context.Pedidos.Add(pedido);
            _context.SaveChanges(); // Salva o pedido antes de adicionar itens

            // Passo 3: Adicionar itens ao pedido
            var carrinho = _context.CarrinhoCompras
                .Include(c => c.Peca)
                .Where(c => c.IdCliente == model.IdCliente)
                .ToList();

            if (carrinho.Any())
            {
                decimal totalPedido = 0;

                foreach (var item in carrinho)
                {
                    var itemPedido = new ItensPedido
                    {
                        IdPedido = pedido.IdPedido,
                        IdPeca = item.IdPeca,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = (decimal)item.Peca.Preco,
                    };

                    _context.ItensPedido.Add(itemPedido);
                    totalPedido += itemPedido.Quantidade * itemPedido.PrecoUnitario;
                }

                // Passo 4: Atualizar o total do pedido
                pedido.Total = Math.Round(totalPedido, 2);
                _context.Pedidos.Update(pedido);
                _context.SaveChanges();

                // Passo 5: Limpar o carrinho após finalização da compra
                _context.CarrinhoCompras.RemoveRange(carrinho);
                _context.SaveChanges();
            }

            TempData["Sucesso"] = "Pedido concluído com sucesso!";
            return RedirectToAction("ResumoPedido", new { idPedido = pedido.IdPedido });
        }

        public IActionResult ResumoPedido(int idPedido)
        {
            var pedido = _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Peca)
                .FirstOrDefault(p => p.IdPedido == idPedido);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        public IActionResult FinalizarCompra(int idCliente)
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<CarrinhoCompras>>("Carrinho") ?? new List<CarrinhoCompras>();
            if (!carrinho.Any()) return RedirectToAction("Cesta");

            // 🔥 Validar se há estoque suficiente
            foreach (var item in carrinho)
            {
                var peca = _context.Pecas.Find(item.IdPeca);
                if (peca == null || item.Quantidade > peca.Stock)
                {
                    TempData["Erro"] = $"A peça {peca?.Nome ?? "desconhecida"} não tem estoque suficiente!";
                    return RedirectToAction("Cesta");
                }
            }

            // 🔥 Criar o pedido na tabela `Pedidos`
            var novoPedido = new Pedidos
            {
                IdCliente = idCliente,
                DataCompra = DateTime.Now,
                Total = (decimal)carrinho.Sum(item => item.Quantidade * (double)_context.Pecas.Find(item.IdPeca).Preco),
                Status = "Pendente"
            };

            _context.Pedidos.Add(novoPedido);
            _context.SaveChanges();

            // 🔥 Adicionar itens na tabela `ItensPedido` e atualizar estoque
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

                // 🔥 Buscar anúncio associado à peça
                var anuncio = _context.AnuncioPecas.FirstOrDefault(a => a.IdAnuncioPeca == peca.IdPeca);

                // 🔥 Registrar venda na tabela `VendaPeca`
                _context.VendaPeca.Add(new VendaPeca
                {
                    IdAnuncio = anuncio?.IdAnuncioPeca ?? 0, // 🔥 Corrige o acesso ao ID do anúncio
                    Quantidade = item.Quantidade,
                    PrecoUnitario = (decimal)peca.Preco,
                    DataVenda = DateTime.Now
                });

                // 🔥 Atualizar estoque
                peca.Stock -= item.Quantidade;

                // 🔥 Se esgotar, marcar peça como vendida no anúncio
                if (peca.Stock <= 0 && anuncio != null)
                {
                    anuncio.Vendido = true;
                }
            }

            _context.SaveChanges();

            // 🔥 Limpar o carrinho após finalizar compra
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("ResumoPedido", new { idPedido = novoPedido.IdPedido });
        }
    }
}
