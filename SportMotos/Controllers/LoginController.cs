using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        //Vai buscar o formulário
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost] // 🔥 Agora é POST, pois envia dados sensíveis
        public async Task<IActionResult> Login(string Email, string password)
        {
            // Primeiro, tenta encontrar um Cliente com esse email
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == Email);

            // Verifica se há algum administrador com esse email
            var admin = _context.Admins.FirstOrDefault(a => a.Email == Email);

            System.Diagnostics.Debug.WriteLine($"Cliente: {cliente?.Nome}, Admin: {admin?.Nome}");

            if (cliente == null && admin == null)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Verifica se o User correspondente ao Cliente/Admin existe na tabela Users
            var username = cliente?.Nome ?? admin?.Nome; // Nome do utilizador (cliente ou admin)
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Password != password)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Criar os Claims (dados da sessão)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Tipo_Utilizador", user.Tipo_Utilizador) // "Cliente" ou "Admin"
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Criar a sessão de autenticação
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            System.Diagnostics.Debug.WriteLine($"✅ Login feito com sucesso para {user.Username}");

            // Redirecionar consoante o tipo de utilizador
            if (user.Tipo_Utilizador == "Cliente")
            {
                System.Diagnostics.Debug.WriteLine($"A redirecionar index");
                return RedirectToAction("Index", "Home"); // Cliente vai para página inicial
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"A redirecionar dashboard");
                return RedirectToAction("DashBoard", "Dashboard"); // Admin vai para dashboard
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login"); // Redireciona para a tela de login
        }
    }
}
