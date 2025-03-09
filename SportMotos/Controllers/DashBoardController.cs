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

        public IActionResult Dashboard()
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TotalClientes = _context.Clientes.Count();
            ViewBag.TotalUsuarios = _context.Users.Count();
            ViewBag.TotalAnunciosMoto = _context.AnuncioMotos.Count();
            ViewBag.TotalAnunciosPeca = _context.AnuncioPecas.Count();

            ViewBag.Motos = _context.Motos;
            ViewBag.Pecas = _context.Pecas;

            ViewBag.TotalVendasMes = _context.Pedidos
                .Where(p => p.DataCompra.Month == DateTime.Now.Month &&
                            p.DataCompra.Year == DateTime.Now.Year)
                .Sum(p => (decimal?)p.Total) ?? 0;

            ViewBag.PedidosPendentes = _context.Pedidos.Count(p => p.Status == "Pendente");

            var ultimoCliente = _context.Clientes
                .OrderByDescending(c => c.DataCriacao)
                .Select(c => c.Nome)
                .FirstOrDefault();
            ViewBag.UltimoCliente = ultimoCliente ?? "Nenhum Cliente";

            // 🔥 Buscar os 5 pedidos mais recentes
            ViewBag.UltimosPedidos = _context.Pedidos
                .Include(p=>p.Cliente)
                .OrderByDescending(p => p.DataCompra)
                .Take(5)
                .Select(p => new
                {
                    p.IdPedido,
                    ClienteNome = p.Cliente.Nome, // Obtém o nome do Cliente
                    p.DataCompra,
                    p.Status
                })
                .ToList();

            // Buscar os 5 anúncios de motos mais recentes
            ViewBag.UltimosAnunciosMoto = _context.AnuncioMotos
                .Include(a => a.IdMotoNavigation)
                .OrderByDescending(a => a.DataPublicacao)
                .Take(5)
                .Select(a => new
                {
                    a.IdAnuncioMoto,
                    a.Titulo,
                    a.DataPublicacao,
                    a.Preco,
                    Moto = a.IdMotoNavigation.Modelo
                })
                .ToList();

            // Buscar os 5 anúncios de peças mais recentes
            ViewBag.UltimosAnunciosPeca = _context.AnuncioPecas
                .Include(a => a.IdPecaNavigation)
                .OrderByDescending(a => a.DataPublicacao)
                .Take(5)
                .Select(a => new
                {
                    a.IdAnuncioPeca,
                    a.Titulo,
                    a.DataPublicacao,
                    a.Preco,
                    Peca = a.IdPecaNavigation.Nome
                })
                .ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEstatisticas(int mes)
        {
            int clientes = await _context.Users.Where(u => u.Data_Criacao.Month == mes).CountAsync();
            int anunciosMotoVendidos = await _context.AnuncioMotos.Where(a => a.DataVenda.Month == mes).CountAsync();
            int anuncioPecasVendidos = await _context.AnuncioPecas.Where(p=> p.DataVenda.Month == mes).CountAsync();

            return Json(new { mes, clientes, anunciosMotoVendidos, anuncioPecasVendidos });
        }

    }
}
