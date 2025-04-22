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
            Console.WriteLine("🚀 Método Login acionado!");

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("❌ Erro: E-mail ou senha em branco.");
                ViewBag.Mensagem = "E-mail e senha são obrigatórios!";
                return View();
            }

            var emailNormalizado = Email.Trim().ToLower();
            Console.WriteLine($"🔍 E-mail recebido: {Email}");
            Console.WriteLine($"🔍 E-mail normalizado: {emailNormalizado}");

            // 🔍 Verifica se é um Cliente
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email.Trim().ToLower() == emailNormalizado);
            Console.WriteLine(cliente != null ? $"✅ Cliente encontrado: {cliente.Nome}, ID: {cliente.IdCliente}" : "❌ Cliente não encontrado.");

            // 🔍 Verifica se é um Administrador
            var admin = _context.Admins.FirstOrDefault(a => a.Email.Trim().ToLower() == emailNormalizado);
            Console.WriteLine(admin != null ? $"✅ Admin encontrado: {admin.Nome}, ID: {admin.IdAdmin}" : "❌ Admin não encontrado.");

            if (cliente == null && admin == null)
            {
                Console.WriteLine("❌ Erro: Nenhum cliente ou admin encontrado.");
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // Obtém o nome do utilizador
            var username = cliente?.Nome ?? admin?.Nome;
            Console.WriteLine($"🔍 Nome do utilizador identificado: {username}");

            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("❌ Erro: Nome de utilizador não encontrado.");
                ViewBag.Mensagem = "Erro ao identificar o utilizador!";
                return View();
            }

            // Busca o utilizador na tabela Users
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            Console.WriteLine(user != null ? $"✅ Usuário encontrado na tabela Users: {user.Username}" : "❌ Usuário não encontrado na tabela Users.");

            if (user == null)
            {
                ViewBag.Mensagem = "E-mail ou senha inválidos!";
                return View();
            }

            // 🔐 Verifica a senha com BCrypt
            bool senhaValida;
            Console.WriteLine($"🔍 Senha digitada: {password}");
            Console.WriteLine($"🔍 Senha armazenada: {user.Password}");

            try
            {
                senhaValida = BCrypt.Net.BCrypt.Verify(password, user.Password.Trim());
                Console.WriteLine($"🔄 Comparação da senha: {senhaValida}");
            }
            catch (SaltParseException ex)
            {
                Console.WriteLine($"❌ Erro ao verificar a senha: {ex.Message}");
                ViewBag.Mensagem = "Erro ao verificar a senha. Tente novamente.";
                return View();
            }

            if (!senhaValida)
            {
                Console.WriteLine("❌ Erro: Senha inválida.");
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
                claims.Add(new Claim("IdCliente", cliente.IdCliente.ToString()));
                Console.WriteLine($"✅ Cliente autenticado: {cliente.Nome}, ID: {cliente.IdCliente}");
            }

            // Adiciona o IdAdmin como claim se for Admin
            if (admin != null)
            {
                claims.Add(new Claim("IdAdmin", admin.IdAdmin.ToString()));
                Console.WriteLine($"✅ Admin autenticado: {admin.Nome}, ID: {admin.IdAdmin}");
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // 🔒 Criar sessão persistente
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            Console.WriteLine("✅ Login realizado com sucesso!");

            // 🔄 Redireciona conforme o tipo de utilizador
            if (cliente != null)
            {
                Console.WriteLine("🔄 Redirecionando Cliente para Home.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Console.WriteLine("🔄 Redirecionando Admin para Dashboard.");
                return RedirectToAction("Dashboard", "DashBoard");
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
