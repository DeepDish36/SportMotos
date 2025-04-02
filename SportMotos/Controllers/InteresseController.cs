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
            var moto = _context.Motos.FirstOrDefault(m => m.IdMoto == idMoto);
            if (moto == null)
            {
                return NotFound();
            }

            var model = new InteresseMotos
            {
                IdCliente = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), // ID do cliente logado
                IdMoto = idMoto,
                DataInteresse = DateTime.Now,
                Status = "Pendente"
            };

            return PartialView("_FormularioInteresse", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SalvarInteresse(InteresseMotos interesse)
        {
            if (ModelState.IsValid)
            {
                _context.InteresseMotos.Add(interesse);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Erro ao salvar interesse." });
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
