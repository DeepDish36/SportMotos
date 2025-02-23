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
            // Verifica se o e-mail existe na tabela Clientes
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == Email);

            if (cliente == null)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Verifica se o cliente tem um utilizador correspondente na tabela Users
            var user = _context.Users.FirstOrDefault(u => u.Username == cliente.Nome);

            if (user == null || user.Password != password) // 🔥 Lembrar de adicionar Hashing depois!
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Criar os Claims (dados da sessão)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("TipoUtilizador", user.Tipo_Utilizador) // Pode ser "Cliente" ou "Admin"
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
                return RedirectToAction("Dashboard", "Dashboard"); // Admin vai para dashboard
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login"); // Redireciona para a tela de login
        }
    }
}
