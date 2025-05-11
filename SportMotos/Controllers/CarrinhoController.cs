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

            // Usar SQL Raw para inserir (com EF não funciona)
            string sqlInsertPedido = @"
    INSERT INTO Pedidos (ID_Cliente, DataCompra, Status, Total)
    OUTPUT INSERTED.ID_Pedido
    VALUES (@p0, @p1, @p2, @p3)";

            int idPedido = _context.Database.ExecuteSqlRaw(sqlInsertPedido,
                idCliente, DateTime.Now, "Pendente", 0);

            //Adiciona itens
            decimal totalPedido = 0;

            foreach (var item in carrinho)
            {
                string sqlInsertItem = @"
        INSERT INTO ItensPedido (ID_Pedido, ID_Peca, Quantidade, PrecoUnitario)
        VALUES (@p0, @p1, @p2, @p3)";

                _context.Database.ExecuteSqlRaw(sqlInsertItem,
                    idPedido, item.IdPeca, item.Quantidade, (decimal)item.Peca.Preco);

                totalPedido += item.Quantidade * (decimal)item.Peca.Preco;
            }

            //Atualizar total do pedido após inserir
            string sqlUpdatePedido = @"
    UPDATE Pedidos SET Total = @p0 WHERE ID_Pedido = @p1";

            _context.Database.ExecuteSqlRaw(sqlUpdatePedido, Math.Round(totalPedido, 2), idPedido);

            //Limpar carrinho
            string sqlDeleteCarrinho = @"
    DELETE FROM CarrinhoCompras WHERE ID_Cliente = @p0";

            _context.Database.ExecuteSqlRaw(sqlDeleteCarrinho, idCliente);

            TempData["Sucesso"] = "Pedido concluído com sucesso!";
            return RedirectToAction("ResumoPedido", new { idPedido = idPedido });
        }

        public IActionResult ResumoPedido(int idPedido)
        {
            string sqlGetPedido = @"SELECT * FROM Pedidos WHERE ID_Pedido = @p0";

            var pedido = _context.Pedidos.FromSqlRaw(sqlGetPedido, idPedido).FirstOrDefault();

            if (pedido == null) return NotFound();

            string sqlGetItens = @"SELECT * FROM ItensPedido WHERE ID_Pedido = @p0";

            var itensPedido = _context.ItensPedido.FromSqlRaw(sqlGetItens, idPedido).ToList();

            foreach (var item in itensPedido)
            {
                string sqlGetPeca = @"SELECT * FROM Peca WHERE ID_Peca = @p0";

                item.Peca = _context.Pecas.FromSqlRaw(sqlGetPeca, item.IdPeca).FirstOrDefault();
            }

            // ✅ Associar os itens ao pedido manualmente
            pedido.Itens = itensPedido;

            return View(pedido);
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
