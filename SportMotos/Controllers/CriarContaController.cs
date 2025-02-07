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

        // Exibe o formulário de registro (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Processa o formulário de registro (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Cliente model)
        {
            if (ModelState.IsValid)
            {
                // Adiciona o novo cliente ao banco de dados
                model.DataCriacao = DateTime.Now;
                _context.Clientes.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Redireciona após o cadastro
            }

            return View(model);
        }
    }
}
