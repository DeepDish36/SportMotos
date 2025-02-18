using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    //Controller para criar um anúncio
    public class AnuncioMotoController : Controller
    {
        private readonly AppDbContext _context;

        public AnuncioMotoController(AppDbContext context)
        {
            _context = context;
        }

        //Listar anúncios de motos (GET)
        public async Task<IActionResult> AnuncioMoto(int id)
        {
            var anuncioMoto = await _context.AnuncioMotos.FindAsync(id);
            if (anuncioMoto == null) return NotFound();
            return View(anuncioMoto);
        }


        //Formulário de adição de anúncio de moto (GET)
        public IActionResult AdicionarAnuncioMoto()
        {
            return View();
        }

        //Adicionar anúncio de moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarAnuncioMoto(AnuncioMoto anuncioMoto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(anuncioMoto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AnuncioMoto));
            }
            return View(anuncioMoto);
        }


        //Formulário de edição do anúncio de moto (GET)
        public async Task<IActionResult> EditarAnuncioMoto(int id)
        {
            var anuncioMoto = await _context.AnuncioMotos.FindAsync(id);
            if (anuncioMoto == null) return NotFound();
            return View(anuncioMoto);
        }


        //Editar anúncio de moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAnuncioMoto(AnuncioMoto anuncioMoto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(anuncioMoto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AnuncioMoto));
            }
            return View(anuncioMoto);
        }


        //Apagar anúncio de moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarAnuncioMoto(int id)
        {
            var anuncioMoto = await _context.AnuncioMotos.FindAsync(id);
            if (anuncioMoto == null) return NotFound();
            anuncioMoto.ApagadoEm = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AnuncioMoto));
        }

        //Excluir AnuncioMoto (GET)
        public async Task<IActionResult> ExcluirAnuncioMoto(int id)
        {
            var anuncioMoto = await _context.AnuncioMotos.FindAsync(id);
            if (anuncioMoto == null) return NotFound();
            return View(anuncioMoto);
        }

        //Confirmar excluir anunciomoto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExcluirAnuncioMoto(int id)
        {
            var anuncioMoto = await _context.AnuncioMotos.FindAsync(id);
            if (anuncioMoto == null) return NotFound();
            _context.AnuncioMotos.Remove(anuncioMoto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AnuncioMoto));
        }
    }
}