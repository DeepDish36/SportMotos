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

        public IActionResult AdicionarAoCarrinho(int idPeca)
        {
            // 🔥 Buscar a peça no banco de dados
            var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == idPeca);
            if (peca == null) return NotFound();

            // 🔥 Recuperar carrinho atual ou criar um novo
            var carrinho = HttpContext.Session.GetObjectFromJson<List<CarrinhoCompras>>("Carrinho") ?? new List<CarrinhoCompras>();

            // 🔥 Adicionar peça ao carrinho
            var itemExistente = carrinho.FirstOrDefault(i => i.IdPeca == idPeca);
            if (itemExistente != null)
            {
                itemExistente.Quantidade++;
            }
            else
            {
                carrinho.Add(new CarrinhoCompras
                {
                    IdPeca = peca.IdPeca,
                    Quantidade = 1
                });
            }

            // 🔥 Salvar carrinho atualizado na sessão
            HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);

            return RedirectToAction("Cesta");
        }

        public IActionResult Cesta()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<CarrinhoCompras>>("Carrinho") ?? new List<CarrinhoCompras>();
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
                Total = carrinho.Sum(item => item.Quantidade * _context.Pecas.Find(item.IdPeca).Preco),
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
                    PrecoUnitario = peca.Preco
                });
            }

            _context.SaveChanges();

            // 🔥 Limpar o carrinho após finalizar compra
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("ResumoPedido", new { idPedido = novoPedido.IdPedido });
        }

        public IActionResult TesteSessao()
        {
            // 🔥 Teste: armazenar um valor na sessão
            HttpContext.Session.SetString("Teste", "Sessão funcionando!");

            // 🔥 Recuperar o valor da sessão
            var valor = HttpContext.Session.GetString("Teste");

            return Content($"Valor salvo na sessão: {valor}");
        }
    }
}
