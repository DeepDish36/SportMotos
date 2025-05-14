using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportMotos.Controllers
{
    [Authorize]
    public class CriarAnuncioController : Controller
    {
        private readonly AppDbContext _context;

        public CriarAnuncioController(AppDbContext context)
        {
            _context = context;
        }

        // Listar anúncios
        [HttpGet]
        public async Task<IActionResult> ShowAnuncios()
        {
            var tipoUtilizador= User.FindFirstValue("Tipo_Utilizador");

            if (tipoUtilizador != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anunciosMotos = await _context.AnuncioMotos.Include(a => a.IdMotoNavigation).ToListAsync();
            var anunciosPecas = await _context.AnuncioPecas.Include(a => a.IdPecaNavigation).ToListAsync();

            ViewBag.AnunciosMotos = anunciosMotos;
            ViewBag.AnunciosPecas = anunciosPecas;
            return View();
        }

        // *--------------------CRUD ANÚNCIOS--------------------* //

        // Criar anúncio de moto
        [HttpGet]
        public IActionResult CriarAnuncioMoto()
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar motos que não estão associadas a um anúncio
            var motosDisponiveis = _context.Motos
                .Where(m => !_context.AnuncioMotos.Any(a => a.IdMoto == m.IdMoto))
                .Select(m => new { m.IdMoto, m.Marca, m.Modelo })
                .ToList();

            // Preencher a ViewBag com as motos disponíveis
            ViewBag.MotosDisponiveis = new SelectList(motosDisponiveis, "IdMoto", "Marca");

            ViewBag.Motos = _context.Motos.ToList(); // Pega todas as motos cadastradas
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarAnuncioMoto(AnuncioMoto anuncio)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                // Captura os erros do ModelState e exibe na ViewBag
                ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
                return View(anuncio);
            }

            anuncio.DataPublicacao = DateTime.Now;
            anuncio.DataExpiracao = DateTime.Now.AddDays(50); // Define a data de expiração para 30 dias a partir da publicação
            anuncio.Visualizacoes = 0;
            anuncio.Favoritos = 0;
            anuncio.Avaliacoes = 0;
            _context.AnuncioMotos.Add(anuncio);
            await _context.SaveChangesAsync();

            // Define uma mensagem de sucesso na ViewBag
            ViewBag.Success = "Anúncio publicado com sucesso!";

            return View(anuncio); // Mantém o usuário na mesma página
        }

        // Criar anúncio de peça
        [HttpGet]
        public IActionResult CriarAnuncioPeca()
        {
            var tipoUtilizador = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUtilizador != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar peças que não estão associadas a um anúncio
            var pecasDisponiveis = _context.Pecas
                .Where(m => !_context.AnuncioPecas.Any(a => a.IdPeca == m.IdPeca))
                .Select(m => new { m.IdPeca, m.Nome, m.Categoria, m.Modelo })
                .ToList();

            // Preencher a ViewBag com as peças disponíveis
            ViewBag.PecasDisponiveis = new SelectList(pecasDisponiveis, "IdPeca", "Nome");

            ViewBag.Pecas = _context.Pecas.ToList(); // Vai buscar todas as peças cadastradas
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> CriarAnuncioPeca(AnuncioPeca anuncio)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                // Captura os erros do ModelState e exibe na ViewBag
                ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
                return View(anuncio);
            }

            anuncio.DataPublicacao = DateTime.Now;
            anuncio.DataExpiracao = DateTime.Now.AddDays(50); // Define a data de expiração para 30 dias a partir da publicação
            anuncio.Visualizacoes = 0;
            anuncio.Favoritos = 0;
            anuncio.Avaliacoes = 0;
            _context.AnuncioPecas.Add(anuncio);
            await _context.SaveChangesAsync();

            // Define uma mensagem de sucesso na ViewBag
            ViewBag.Success = "Anúncio publicado com sucesso!";

            return View(anuncio); // Mantém o usuário na mesma página
        }

        // Editar anúncio
        public async Task<IActionResult> EditarAnuncioMoto(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anuncio = await _context.AnuncioMotos.FindAsync(id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        [HttpPost]
        public async Task<IActionResult> EditarAnuncioMoto(AnuncioMoto anuncio)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var anuncioExistente = await _context.AnuncioMotos.FindAsync(anuncio.IdAnuncioMoto);
                if (anuncioExistente == null)
                {
                    Console.WriteLine("❌ Erro: Anúncio não encontrado.");
                    ViewBag.Error = new List<string> { "O anúncio não existe." };
                    return View(anuncio);
                }

                // Atualiza apenas os campos editáveis
                anuncioExistente.Titulo = anuncio.Titulo;
                anuncioExistente.Descricao = anuncio.Descricao;
                anuncioExistente.DataEdicao = DateTime.Now;

                _context.AnuncioMotos.Update(anuncioExistente);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ Anúncio atualizado com sucesso.");
                ViewBag.Success = "Anúncio atualizado com sucesso!";
                return RedirectToAction("Dashboard", "DashBoard");
            }

            Console.WriteLine("❌ ModelState inválido: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            return View(anuncio);
        }

        [HttpGet]
        public IActionResult EditarAnuncioPeca(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anuncioPeca = _context.AnuncioPecas
                .Include(a => a.IdPecaNavigation)
                .FirstOrDefault(a => a.IdAnuncioPeca == id);

            if (anuncioPeca == null)
            {
                return NotFound();
            }

            ViewBag.PecasDisponiveis = new SelectList(_context.Pecas, "IdPeca", "Nome", anuncioPeca.IdPeca);

            return View(anuncioPeca);
        }

        [HttpPost]
        public async Task<IActionResult> EditarAnuncioPeca(AnuncioPeca model)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState inválido: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                ViewBag.PecasDisponiveis = new SelectList(_context.Pecas, "IdPeca", "Nome", model.IdPeca);
                return View(model);
            }

            var anuncioExistente = await _context.AnuncioPecas.FindAsync(model.IdAnuncioPeca);
            if (anuncioExistente == null)
            {
                Console.WriteLine("Erro: Anúncio não encontrado.");
                ViewBag.Error = new List<string> { "O anúncio não existe." };
                return View(model);
            }

            anuncioExistente.Titulo = model.Titulo;
            anuncioExistente.Descricao = model.Descricao;
            anuncioExistente.DataEdicao = DateTime.Now;

            _context.AnuncioPecas.Update(anuncioExistente);
            await _context.SaveChangesAsync();

            Console.WriteLine("Anúncio atualizado com sucesso.");
            TempData["Sucesso"] = "Anúncio atualizado com sucesso!";

            return RedirectToAction("Dashboard", "DashBoard");
        }

        // Marcar como vendido
        public async Task<IActionResult> MarcarComoVendido(int id, string tipo)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (tipo == "Moto")
            {
                var anuncio = await _context.AnuncioMotos.FindAsync(id);
                if (anuncio != null)
                {
                    anuncio.Vendido = true;
                    await _context.SaveChangesAsync();
                }
            }
            else if (tipo == "Peca")
            {
                var anuncio = await _context.AnuncioPecas.FindAsync(id);
                if (anuncio != null)
                {
                    anuncio.Vendido = true;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // Deletar anúncio
        public async Task<IActionResult> DeletarAnuncioMoto(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar o anúncio com a moto associada
            var anuncio = await _context.AnuncioMotos
                .Include(a => a.IdMotoNavigation) // Inclui a moto, mas não a remove
                .FirstOrDefaultAsync(a => a.IdAnuncioMoto == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            // Remover apenas o anúncio, sem excluir a moto
            _context.AnuncioMotos.Remove(anuncio);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "DashBoard");
        }

        public async Task<IActionResult> DeletarAnuncioPeca(int id)
        {
            var tipoUser = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUser != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var anuncio = await _context.AnuncioPecas
                .Include(a => a.IdPecaNavigation)
                .FirstOrDefaultAsync(a=>a.IdAnuncioPeca == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            // Remover apenas o anúncio, sem excluir a peça
            _context.AnuncioPecas.Remove(anuncio);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "DashBoard");
        }

        // *-----------------------------------------------------* //

        // *--------------------DETALHES--------------------* //
        public IActionResult GetMotoDetails(int id)
        {
            var moto=_context.Motos.Find(id);

            if(moto==null)
            {
                return NotFound();
            }

            var motoDetails = new
            {
                marca = moto.Marca,
                modelo = moto.Modelo,
                preco = moto.Preco,
                condicao = moto.Condicao
            };

            return Json(motoDetails);
        }

        public IActionResult GetPecaDetails(int id)
        {
            var peca = _context.Pecas.Find(id);
            if (peca == null)
            {
                return NotFound();
            }
            var pecaDetails = new
            {
                nome = peca.Nome,
                marca = peca.Marca,
                modelo = peca.Modelo,
                categoria = peca.Categoria,
                preco = peca.Preco,
                condicao = peca.Estado
            };
            return Json(pecaDetails);
        }

        public IActionResult GetAnuncioMotoDetails(int id)
        {
            var anuncio = _context.AnuncioMotos
                .Include(a => a.IdMotoNavigation)
                .FirstOrDefault(a => a.IdMoto == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            var anuncioDetails = new
            {
                titulo = anuncio.Titulo,
                descricao = anuncio.Descricao,
                preco = anuncio.IdMotoNavigation?.Preco,
                marca = anuncio.IdMotoNavigation?.Marca,
                modelo = anuncio.IdMotoNavigation?.Modelo,
                condicao = anuncio.IdMotoNavigation?.Condicao
            };

            return Json(anuncioDetails);
        }

        public JsonResult GetAnuncioPecaDetails(int id)
        {
            var peca = _context.Pecas.FirstOrDefault(p => p.IdPeca == id);

            if (peca == null)
            {
                return Json(new { erro = "Peça não encontrada!" });
            }

            return Json(new
            {
                marca = peca.Marca,
                modelo = peca.Modelo,
                categoria = peca.Categoria,
                preco = peca.Preco,
                condicao = peca.Estado
            });
        }

        // *-----------------------------------------------------* //
    }
}
