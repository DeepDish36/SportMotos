using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class CriarContaController : Controller
    {
        private readonly AppDbContext _context;

        public CriarContaController(AppDbContext context)
        {
            _context = context;
        }

        // Exibe o formulário de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Processa o formulário de registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.DataCriacao = DateTime.Now;
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(cliente);
        }
    }
}
