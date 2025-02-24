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
            var totalClientes = _context.Clientes.Count();
            var totalUtilizadores = _context.Users.Count();
            var totalAnunciosMoto = _context.AnuncioMotos.Count(); // Supondo que haja uma tabela "Anuncios"
            var totalAnunciosPeca = _context.AnuncioPecas.Count();
            
            ViewBag.TotalClientes = totalClientes;
            ViewBag.TotalUtilizadores = totalUtilizadores;
            ViewBag.TotalAnunciosMoto = totalAnunciosMoto;
            ViewBag.TotalAnuncioPecas = totalAnunciosPeca;

            return View();
        }
    }
}
