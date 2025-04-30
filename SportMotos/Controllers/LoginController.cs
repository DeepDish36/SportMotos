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
            Console.WriteLine("Método Login acionado!");

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Erro: E-mail ou senha em branco.");
                ViewBag.Mensagem = "E-mail e senha são obrigatórios!";
                return View();
            }

            var emailNormalizado = Email.Trim().ToLower();

            // 🔍 Verifica se é um Cliente
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email.Trim().ToLower() == emailNormalizado);

            // 🔍 Verifica se é um Administrador
            var admin = _context.Admins.FirstOrDefault(a => a.Email.Trim().ToLower() == emailNormalizado);

            if (cliente == null && admin == null)
            {
                Console.WriteLine("Erro: Nenhum cliente ou admin encontrado.");
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Obtém o nome do utilizador
            var username = cliente?.Nome ?? admin?.Nome;
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Erro: Nome de utilizador não encontrado.");
                ViewBag.Mensagem = "Erro ao identificar o utilizador!";
                return View();
            }

            // Busca o utilizador na tabela Users
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("Erro: Usuário não encontrado na tabela Users.");
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // 🔐 Verifica a senha com BCrypt
            bool senhaValida;
            try
            {
                senhaValida = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (SaltParseException ex)
            {
                Console.WriteLine($"Erro ao verificar a senha: {ex.Message}");
                ViewBag.Mensagem = "Erro ao verificar a senha. Tente novamente.";
                return View();
            }

            if (!senhaValida)
            {
                Console.WriteLine("Erro: Senha inválida.");
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // 🔥 Criar os Claims (dados da sessão)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username), // Nome do utilizador
        new Claim("Tipo_Utilizador", user.Tipo_Utilizador), // Tipo de utilizador (Cliente ou Admin)
        new Claim(ClaimTypes.Email, emailNormalizado) // Email normalizado
    };

            // Adiciona o IdCliente como claim se for Cliente
            if (cliente != null)
            {
                claims.Add(new Claim("IdCliente", cliente.IdCliente.ToString())); // ID do cliente com nome separado
                Console.WriteLine($"Cliente autenticado: {cliente.Nome}, ID: {cliente.IdCliente}");
            }

            // Adiciona o IdAdmin como claim se for Admin
            if (admin != null)
            {
                claims.Add(new Claim("IdAdmin", admin.IdAdmin.ToString())); // ID do admin com nome separado
                Console.WriteLine($"Admin autenticado: {admin.Nome}, ID: {admin.IdAdmin}");
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 🔒 Criar sessão persistente
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantém sessão após fechar navegador
                ExpiresUtc = DateTime.UtcNow.AddHours(1) // Expira em 1 hora
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            Console.WriteLine("Login realizado com sucesso!");

            // 🔄 Redireciona conforme o tipo de utilizador
            if (cliente != null)
            {
                Console.WriteLine("Redirecionando Cliente para Home.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Console.WriteLine("Redirecionando Admin para Dashboard.");
                return RedirectToAction("Dashboard", "DashBoard");
            }
        }

        public JsonResult ObterClienteLogado()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { clienteId = (int?)null, mensagem = "Utilizador não autenticado." });
            }

            return Json(new { clienteId = int.Parse(userIdClaim) });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login"); // Redireciona para a tela de login
        }
    }
}
