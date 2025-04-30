using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using SportMotos.Helpers;

namespace SportMotos.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;

        public CarrinhoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult AdicionarAoCarrinho(int idPeca, int idCliente)
        {
            // 🔥 Buscar a peça no banco de dados
            var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == idPeca);
            if (peca == null) return NotFound();

            // 🔥 Verificar se a peça já está no carrinho do usuário
            var itemExistente = _context.CarrinhoCompras.FirstOrDefault(i => i.IdCliente == idCliente && i.IdPeca == idPeca);

            if (itemExistente != null)
            {
                // 🔥 Se já existe, aumentar a quantidade
                itemExistente.Quantidade++;
            }
            else
            {
                // 🔥 Se não existe, criar um novo item no banco
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

        public JsonResult ObterCarrinho(int idCliente)
        {
            var carrinho = _context.CarrinhoCompras
                .Where(i => i.IdCliente == idCliente) // 🔥 Ajustado para refletir o banco de dados
                .Select(i => new
                {
                    id = i.IdPeca, // 🔥 Corrigido para usar o nome exato do BD
                    name = i.Peca.Nome,
                    brand = i.Peca.Marca,
                    price = i.Peca.Preco,
                    quantity = i.Quantidade,
                    image = "/images/pecas/" + i.IdPeca + ".jpg"
                })
                .ToList();

            return Json(carrinho);
        }

        public IActionResult Cesta(int idCliente)
        {
            var carrinho = _context.CarrinhoCompras.Where(i => i.IdCliente == idCliente).ToList();
            return View(carrinho);
        }

        public IActionResult FinalizarCompra(int idCliente)
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<CarrinhoCompras>>("Carrinho") ?? new List<CarrinhoCompras>();
            if (!carrinho.Any()) return RedirectToAction("Cesta");

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

            // 🔥 Adicionar itens do carrinho na tabela `ItensPedido`
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
            }

            _context.SaveChanges();

            // 🔥 Limpar o carrinho após finalizar compra
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("ResumoPedido", new { idPedido = novoPedido.IdPedido });
        }
    }
}
