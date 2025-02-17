using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // Registra um novo cliente (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Verifica se o usuário já existe na tabela Users
                var usuarioExistente = await _context.Users.FirstOrDefaultAsync(u => u.Username == cliente.Nome);

                if (usuarioExistente == null) // Se não existe, cria um novo
                {
                    usuarioExistente = new User { Username = cliente.Nome };
                    _context.Users.Add(usuarioExistente);
                    await _context.SaveChangesAsync();
                }

                // 🔥 Corrigindo o erro - Vinculando corretamente a FK
                cliente.NomeNavigation = usuarioExistente;

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Login");
            }

            // Log de erros de validação
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(cliente);
        }
    }
}