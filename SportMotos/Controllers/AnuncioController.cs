using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    //Mostrar todos os anúncios
    public class AnuncioController : Controller
    {
        private readonly AppDbContext _context;

        public AnuncioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Anuncio(string tipo)
        {
            var tipoUtilizador = User.FindFirst("Tipo_Utilizador")?.Value;
            ViewBag.TipoUtilizador = tipoUtilizador;

            if (tipo == "motos")
            {
                // Busca anúncios de motos
                var anunciosMotos = await _context.AnuncioMotos
                    .Where(a => a.ApagadoEm == null) // Apenas anúncios ativos
                    .ToListAsync();
                ViewBag.TipoAnuncio = "motos";
                return View(anunciosMotos);
            }
            else if (tipo == "pecas")
            {
                // Busca anúncios de peças
                var anunciosPecas = await _context.AnuncioPecas
                    .Where(a => a.ApagadoEm == null) // Apenas anúncios ativos
                    .Include(a => a.IdPecaNavigation) // Inclui a tabela Peca para obter o Stock
                    .ToListAsync();
                ViewBag.TipoAnuncio = "pecas";
                // Passa os dados para a View
                return View(anunciosPecas);
            }

            // Se o tipo não for especificado, redireciona para uma página de erro ou exibe todos os anúncios
            return RedirectToAction("Index", "Home");
        }
    }
}
