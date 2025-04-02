using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class InteresseController : Controller
    {
        private readonly AppDbContext _context;

        public InteresseController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize] // Garante que só usuários logados acessem
        public IActionResult Criar(int idMoto)
        {
            var model = new InteresseMotos { IdMoto = idMoto };
            return PartialView("_FormularioInteresse", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SalvarInteresse(InteresseMotos interesse)
        {
            if (ModelState.IsValid)
            {
                // Obter o ID do cliente logado
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int idCliente))
                {
                    interesse.IdCliente = idCliente;
                    interesse.DataInteresse = DateTime.Now;
                    _context.InteresseMotos.Add(interesse);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Erro ao obter o ID do cliente." });
            }
            return Json(new { success = false });
        }

        [Authorize]
        public async Task<IActionResult> Aprovar(int id)
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var interesse = await _context.InteresseMotos.FindAsync(id);
            if (interesse == null) return NotFound();

            interesse.Status = "Aprovado";
            await _context.SaveChangesAsync();

            // Enviar email
            //await _emailService.EnviarEmail(interesse.EmailCliente, "Seu interesse foi aprovado!",
            //    $"Olá {interesse.NomeCliente}, seu interesse na moto foi aprovado! Visite-nos para fechar negócio.");

            return RedirectToAction("Dashboard");
        }
    }
}
