using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            if (string.IsNullOrEmpty(emailUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            if (tipoUtilizador == "Admin")
            {
                var admin = _context.Admins.AsNoTracking().FirstOrDefault(a => a.Email == emailUsuario);
                if (admin == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                return View("PerfilAdmin", admin);
            }
            else
            {
                var cliente = _context.Clientes.AsNoTracking().FirstOrDefault(c => c.Email == emailUsuario);
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
            var emailUsuario = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(emailUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == emailUsuario);
            if (cliente == null)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                // Apenas atualiza os campos modificados, mantendo os antigos se não forem preenchidos
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Nome)) cliente.Nome = clienteAtualizado.Nome;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Sobrenome)) cliente.Sobrenome = clienteAtualizado.Sobrenome;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Telefone)) cliente.Telefone = clienteAtualizado.Telefone;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Rua)) cliente.Rua = clienteAtualizado.Rua;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Morada)) cliente.Morada = clienteAtualizado.Morada;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.CodPostal)) cliente.CodPostal = clienteAtualizado.CodPostal;
                if (!string.IsNullOrWhiteSpace(clienteAtualizado.Genero)) cliente.Genero = clienteAtualizado.Genero;
                if (clienteAtualizado.Idade > 0) cliente.Idade = clienteAtualizado.Idade;
                cliente.ReceberNewsletter = clienteAtualizado.ReceberNewsletter;

                // Atualiza a data da última modificação
                cliente.DataEdicao = DateTime.Now;

                // Marca a entidade como modificada
                _context.Entry(cliente).State = EntityState.Modified;

                // Salva as alterações
                _context.SaveChanges();

                ViewBag.Mensagem = "Perfil atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                ViewBag.Mensagem = "Erro ao atualizar o perfil. Tente novamente.";
                Console.WriteLine($"Erro ao atualizar perfil: {ex}");
            }

            return View("Perfil", cliente);
        }
    }
}
