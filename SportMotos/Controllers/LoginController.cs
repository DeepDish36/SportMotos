using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using System.Security.Claims;
using BCrypt.Net;

namespace SportMotos.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Mensagem = "E-mail e senha são obrigatórios!";
                return View();
            }

            var emailNormalizado = Email.Trim().ToLower();

            // Primeiro, tenta encontrar um Cliente com esse email
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email.Trim().ToLower() == emailNormalizado);

            // Verifica se há algum administrador com esse email
            var admin = _context.Admins.FirstOrDefault(a => a.Email.Trim().ToLower() == emailNormalizado);

            if (cliente == null && admin == null)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Obtém o nome do usuário (Cliente ou Admin)
            var username = cliente?.Nome ?? admin?.Nome;
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Buscar usuário na tabela Users
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // 🔐 Verifica se a senha está correta usando BCrypt
            bool senhaValida;
            try
            {
                senhaValida = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (SaltParseException)
            {
                ViewBag.Mensagem = "Erro ao verificar a senha. Por favor, tente novamente.";
                return View();
            }

            if (!senhaValida)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Criar os Claims (dados da sessão)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Tipo_Utilizador", user.Tipo_Utilizador), // "Cliente" ou "Admin"
                new Claim(ClaimTypes.Email, emailNormalizado) // Garante que o email está nos claims
            };

            // Adicionar o IdCliente como claim, se for um cliente
            if (cliente != null)
            {
                claims.Add(new Claim("IdCliente", cliente.IdCliente.ToString()));
            }

            // 🔥 Adicionar o IdAdmin como claim, se for um admin
            if (admin != null)
            {
                claims.Add(new Claim("IdAdmin", admin.IdAdmin.ToString())); // Agora adiciona corretamente!
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 🔥 Criar a sessão persistente
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantém a sessão após fechar o navegador
                ExpiresUtc = DateTime.UtcNow.AddHours(1) // Expira em 1 hora
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Redirecionar consoante o tipo de utilizador
            return user.Tipo_Utilizador == "Cliente"
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Dashboard", "DashBoard");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login"); // Redireciona para a tela de login
        }
    }
}
