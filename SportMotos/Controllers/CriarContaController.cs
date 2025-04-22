using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using BCrypt;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.CodeAnalysis.Scripting;

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
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(cliente);
            }

            // Verifica se o user já existe na tabela Users
            var userExiste = await _context.Users.AnyAsync(u => u.Username == cliente.Nome);

            if (userExiste)
            {
                ViewBag.Error = "Esse nome já está a ser utilizado!";
                return View(cliente);
            }

            // Gera o hash da senha
            string passwordCrypt = BCrypt.Net.BCrypt.HashPassword(cliente.Password);

            // Cria o novo usuário
            var novoUser = new User
            {
                Username = cliente.Nome,
                Password = passwordCrypt, // 🔥 Senha segura
                Tipo_Utilizador = "Cliente", // Corrigido para o nome correto do banco
                Data_Criacao = DateTime.Now,  // Garante data válida
                Ultimo_Login = DateTime.Now
            };

            _context.Users.Add(novoUser);
            await _context.SaveChangesAsync();

            // Associa o cliente ao user criado
            cliente.NomeNavigation = novoUser;

            // Define o status como "Ativo"
            cliente.Status = "Ativo";

            // Atualiza a senha do cliente com o hash
            cliente.Password = passwordCrypt;

            // Garante que o campo ReceberNewsletter seja true se a checkbox foi marcada
            cliente.ReceberNewsletter = cliente.ReceberNewsletter ? true : false;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Conta criada com sucesso!";
            return RedirectToAction("Login", "Login");
        }
    }
}
