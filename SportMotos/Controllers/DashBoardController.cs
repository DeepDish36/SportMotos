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
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anuncioMotos = _context.AnuncioMotos
                .Where(a => a.DataExpiracao.HasValue)
                .ToList();

            ViewBag.AnuncioMotosExpirados = anuncioMotos.Count(a => (DateTime.Now - a.DataExpiracao.Value).TotalDays >= 50);
            
            var anuncioPecas = _context.AnuncioPecas
                .Where(a => a.DataExpiracao.HasValue)
                .ToList();

            ViewBag.AnuncioPecasExpirados = anuncioPecas.Count(a => (DateTime.Now - a.DataExpiracao.Value).TotalDays >= 50);

            ViewBag.AnunciosProximosDeExpirar = _context.AnuncioPecas
                .Count(a => a.DataExpiracao > DateTime.Now && a.DataExpiracao <= DateTime.Now.AddDays(5));

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

            // 🔹 Buscar os pedidos recentes
            ViewBag.UltimosPedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.DataCompra)
                .Take(5)
                .Select(p => new
                {
                    p.IdPedido,
                    ClienteNome = p.Cliente.Nome,
                    p.DataCompra,
                    p.Status
                })
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
