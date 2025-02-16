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
                // Verifica se o nome já existe na tabela Users
                var usuarioExistente = await _context.Users.FindAsync(cliente.Nome);

                if (usuarioExistente == null) // Se o nome não existe, cria um novo usuário
                {
                    var novoUser = new User { Username = cliente.Nome };
                    _context.Users.Add(novoUser);
                    await _context.SaveChangesAsync();
                }

                // Agora adiciona o cliente, vinculando ao nome que já está na tabela Users
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Login");
            }

            // Adiciona os erros de validação ao ViewBag para exibição
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return View(cliente);
        }
    }
}
