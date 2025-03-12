using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class PecaController : Controller
    {
        //Controller para criar uma moto
        private readonly AppDbContext _context;

        public PecaController(AppDbContext context)
        {
            _context = context;
        }

        //Listar peças
        public async Task<IActionResult> ListarPecas()
        {
            var pecas = await _context.Pecas.ToListAsync();
            return View(pecas);
        }

        //Detalhes da peça
        public async Task<IActionResult> DetalhesMoto(int id)
        {
            var pecas = await _context.Pecas.FindAsync(id);
            if (pecas == null) return NotFound();
            return View(pecas);
        }

        //Formulário de adição da peça ao sistema
        [HttpGet]
        public IActionResult AdicionarPeca()
        {
            return View();
        }

        // Adicionar peça ao sistema (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarPeca(Peca peca, List<IFormFile> Imagens)
        {
            if (!ModelState.IsValid)
            {
                return View(peca);
            }

            // Adiciona a peça ao banco de dados
            _context.Pecas.Add(peca);
            await _context.SaveChangesAsync(); // Salva primeiro para gerar o ID

            // Processa as imagens
            if (Imagens != null && Imagens.Count > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pecas");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var imagem in Imagens)
                {
                    if (imagem.Length > 0)
                    {
                        string fileName = $"{peca.IdPeca}_{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(stream);
                        }

                        // Salva a imagem no banco de dados associada à peça
                        _context.Imagens.Add(new Imagem
                        {
                            NomeArquivo = fileName,
                            PecaId = peca.IdPeca
                        });
                    }
                }

                await _context.SaveChangesAsync(); // Salva as imagens no banco
            }

            ViewBag.Sucesso = "Peça adicionada com sucesso!";
            return RedirectToAction(nameof(ListarPecas));
        }
    }
}
