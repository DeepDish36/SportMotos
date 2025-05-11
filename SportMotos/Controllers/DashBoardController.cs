using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard(bool mostrarTodos = false)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar orçamentos
            var orcamentosQuery = _context.Orcamentos
                .Include(o => o.IdClienteNavigation) // Inclui informações do cliente
                .OrderByDescending(o => o.DataCriacao);

            var orcamentos = orcamentosQuery
                .Select(o => new
                {
                    o.IdOrcamento,
                    ClienteNome = o.IdClienteNavigation.Nome,
                    ClienteEmail = o.IdClienteNavigation.Email,
                    o.DataCriacao,
                    o.Status,
                    o.ValorTotal,
                    o.DetalhesVisualizados
                })
                .ToList();

            ViewBag.Orcamentos = orcamentos;

            // Dados existentes no método
            ViewBag.TotalClientes = _context.Clientes.Count();
            ViewBag.TotalUsuarios = _context.Users.Count();
            ViewBag.TotalAnunciosMoto = _context.AnuncioMotos.Count();
            ViewBag.TotalAnunciosPeca = _context.AnuncioPecas.Count();
            ViewBag.TotalVendasMes = _context.Pedidos
                .Where(p => p.DataCompra.Month == DateTime.Now.Month &&
                            p.DataCompra.Year == DateTime.Now.Year)
                .Sum(p => (decimal?)p.Total) ?? 0;
            ViewBag.PedidosPendentes = _context.Pedidos.Count(p => p.Status == "Pendente");

            ViewBag.Motos = _context.Motos;
            ViewBag.Pecas = _context.Pecas;

            var ultimoCliente = _context.Clientes
                .OrderByDescending(c => c.DataCriacao)
                .Select(c => c.Nome)
                .FirstOrDefault();
            ViewBag.UltimoCliente = ultimoCliente ?? "Nenhum Cliente";

            var ultimosPedidos = _context.InteresseMotos
                .Include(i => i.Cliente) // ✅ Inclui informações do cliente
                .Select(i => new
                {
                    IdPedido = i.IdInteresse,
                    ClienteNome = i.Cliente.Nome, // ✅ Agora pega o nome correto
                    DataCompra = (DateTime?)i.DataInteresse,
                    Total = (decimal?)null,
                    Status = i.Status,
                    TipoPedido = "motos"
                })
                .ToList();

            var ultimosPedidosPeca = _context.Pedidos
                .Include(p => p.Cliente) // ✅ Inclui informações do cliente
                .Select(p => new
                {
                    IdPedido = p.IdPedido,
                    ClienteNome = p.Cliente.Nome, // ✅ Agora pega o nome correto
                    DataCompra = (DateTime?)p.DataCompra,
                    Total = (decimal?)p.Total,
                    Status = p.Status,
                    TipoPedido = "pecas"
                })
                .ToList();

            // ✅ Agora podemos concatenar porque ambos têm a mesma estrutura
            ViewBag.UltimosPedidos = ultimosPedidos.Concat(ultimosPedidosPeca)
                .OrderByDescending(p => p.DataCompra)
                .ToList();

            // 🔹 Buscar anúncios de motos (5 ou todos)
            var anunciosMotoQuery = _context.AnuncioMotos
                .Include(a => a.IdMotoNavigation)
                .OrderByDescending(a => a.DataPublicacao);

            if (!mostrarTodos)
            {
                anunciosMotoQuery = (IOrderedQueryable<AnuncioMoto>)anunciosMotoQuery.Take(5);
            }

            ViewBag.UltimosAnunciosMoto = anunciosMotoQuery
                .Select(a => new
                {
                    a.IdAnuncioMoto,
                    a.Titulo,
                    a.DataPublicacao,
                    a.Preco,
                    a.DataExpiracao,
                    Moto = a.IdMotoNavigation.Modelo
                })
                .ToList();

            // 🔹 Buscar anúncios de peças (5 ou todos)
            var anunciosPecaQuery = _context.AnuncioPecas
                .Include(a => a.IdPecaNavigation)
                .OrderByDescending(a => a.DataPublicacao);

            if (!mostrarTodos)
            {
                anunciosPecaQuery = (IOrderedQueryable<AnuncioPeca>)anunciosPecaQuery.Take(5);
            }

            ViewBag.UltimosAnunciosPeca = anunciosPecaQuery
                .Select(a => new
                {
                    a.IdAnuncioPeca,
                    a.Titulo,
                    a.DataPublicacao,
                    a.Preco,
                    a.DataExpiracao,
                    Peca = a.IdPecaNavigation.Nome
                })
                .ToList();

            ViewBag.MostrarTodos = mostrarTodos; // Passamos essa variável para a View

            ViewBag.Noticias = _context.Noticia.ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEstatisticas(int mes)
        {
            int clientes = await _context.Users.Where(u => u.Data_Criacao.Month == mes).CountAsync();
            int anunciosMotoVendidos = await _context.AnuncioMotos
            .Where(a => a.DataVenda.HasValue && a.DataVenda.Value.Month == mes)
            .CountAsync();

            int anuncioPecasVendidos = await _context.AnuncioPecas
            .Where(p => p.DataVenda.HasValue && p.DataVenda.Value.Month == mes)
            .CountAsync();

            return Json(new { mes, clientes, anunciosMotoVendidos, anuncioPecasVendidos });
        }
    }
}
