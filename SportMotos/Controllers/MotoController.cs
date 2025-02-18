using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class MotoController : Controller
    {

        private readonly AppDbContext _context;

        public MotoController(AppDbContext context)
        {
            _context = context;
        }

        //Listar motos
        public async Task<IActionResult> ListarMotos()
        {
            var motos = await _context.Motos.ToListAsync();
            return View(motos);
        }

        //Detalhes da moto
        public async Task<IActionResult> DetalhesMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            return View(moto);
        }

        //Formulário de adição da moto ao sistema
        public IActionResult AdicionarMoto()
        {
            return View();
        }

        //Adicionar moto ao sistema (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarMoto(Moto moto, IFormFile Imagem)
        {
            if (ModelState.IsValid)
            {
                if (Imagem != null && Imagem.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagem.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Imagens", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagem.CopyToAsync(stream);
                    }

                    // Criar um objeto Imagem
                    var novaImagem = new Imagem
                    {
                        NomeArquivo= fileName,
                        Caminho = "/Motos/" + fileName
                    };

                    // Adiciona a imagem ao banco
                    _context.Imagens.Add(novaImagem);
                    await _context.SaveChangesAsync();

                    // Associa a imagem à moto
                    moto.IdMoto = novaImagem.Id;
                }

                _context.Motos.Add(moto);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(moto);
        }

        //Formulário de edição da moto (GET)
        public async Task<IActionResult> EditarMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            return View(moto);
        }

        //Editar moto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarMoto(Moto moto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(moto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListarMotos));
            }
            return View(moto);
        }

        //Excluir moto (GET)
        public async Task<IActionResult> ExcluirMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();
            return View(moto);
        }

        //Confirmar Excluir moto (POST)
        [HttpPost, ActionName("ExcluirMoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusaoMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if(moto!=null)
            {
                _context.Motos.Remove(moto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarMotos));
        }
    }
}
