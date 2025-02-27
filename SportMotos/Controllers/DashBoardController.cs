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
            // Obtém o tipo de usuário autenticado
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            // Verifica se é administrador
            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home"); // Redireciona se não for admin
            }
            // Total de clientes
            ViewBag.TotalClientes = _context.Clientes.Count();

            // Total de usuários
            ViewBag.TotalUsuarios = _context.Users.Count();

            // Total de anúncios
            ViewBag.TotalAnunciosMoto = _context.AnuncioMotos.Count();

            // Total de vendas no mês atual
            ViewBag.TotalVendasMes = _context.Pedidos
                .Where(p => p.DataCompra.Month == DateTime.Now.Month &&
                            p.DataCompra.Year == DateTime.Now.Year)
                .Sum(p => (decimal?)p.Total) ?? 0; // Evita erro se não houver vendas

            // Total de pedidos pendentes
            ViewBag.PedidosPendentes = _context.Pedidos.Count(p => p.Status == "Pendente");

            // Último cliente cadastrado
            var ultimoCliente = _context.Clientes
                .OrderByDescending(c => c.DataCriacao)
                .Select(c => c.Nome)
                .FirstOrDefault();
            ViewBag.UltimoCliente = ultimoCliente ?? "Nenhum Cliente";

            return View();
        }
    }
}
