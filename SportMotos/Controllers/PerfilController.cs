using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly AppDbContext _context;

        public PerfilController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ShowPerfil()
        {
            var emailUsuario = User.FindFirstValue(ClaimTypes.Email);
            var tipoUtilizador = User.FindFirstValue("Tipo_Utilizador");

            if (emailUsuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (tipoUtilizador == "Admin")
            {
                var admin = _context.Admins.FirstOrDefault(a => a.Email == emailUsuario);
                if (admin == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                return View("PerfilAdmin", admin); // Criar uma view específica para Admins
            }
            else
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Email == emailUsuario);
                if (cliente == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                return View("Perfil", cliente);
            }
        }


        [HttpPost]
        public IActionResult AtualizarPerfil([FromForm] Cliente clienteAtualizado)
        {
            if (!ModelState.IsValid)
            {
                return View("Perfil", clienteAtualizado);
            }

            var emailUsuario = User.FindFirstValue(ClaimTypes.Email);
            if (emailUsuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == emailUsuario);
            if (cliente == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Atualiza os campos
            cliente.Nome = clienteAtualizado.Nome;
            cliente.Sobrenome = clienteAtualizado.Sobrenome;
            cliente.Telefone = clienteAtualizado.Telefone;
            cliente.Rua = clienteAtualizado.Rua;
            cliente.Morada = clienteAtualizado.Morada;
            cliente.CodPostal = clienteAtualizado.CodPostal;
            cliente.Genero = clienteAtualizado.Genero;
            cliente.Idade = clienteAtualizado.Idade;
            cliente.ReceberNewsletter = clienteAtualizado.ReceberNewsletter;

            // Atualiza a data da última modificação
            cliente.DataEdicao = DateTime.Now;

            try
            {
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                ViewBag.Mensagem = "Perfil atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                ViewBag.Mensagem = "Erro ao atualizar o perfil. Tente novamente.";
                Console.WriteLine($"Erro ao atualizar perfil: {ex.Message}");
            }

            return View("Perfil", cliente);
        }
    }
}
