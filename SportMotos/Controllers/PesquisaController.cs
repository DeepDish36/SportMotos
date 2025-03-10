using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class PesquisaController : Controller
    {
        private readonly AppDbContext _context;

        public PesquisaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Pesquisa(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return View(new PesquisaViewModel()); // Retorna um ViewModel vazio se não houver termo
            }

            var anunciosMotos = await _context.Motos
                .Where(m => m.Marca.Contains(termo) || m.Modelo.Contains(termo))
                .ToListAsync();

            var anunciosPecas = await _context.Pecas
                .Where(p => p.Nome.Contains(termo) || p.Marca.Contains(termo))
                .ToListAsync();

            var noticias = await _context.Noticia
                .Where(n => n.Titulo.Contains(termo) || n.Descricao.Contains(termo))
                .ToListAsync();

            var foruns = await _context.Forums
                .Where(f => f.Titulo.Contains(termo) || f.Descricao.Contains(termo))
                .ToListAsync();

            var resultadoPesquisa = new PesquisaViewModel
            {
                Motos = anunciosMotos,
                Pecas = anunciosPecas,
                Noticias = noticias,
                Foruns = foruns
            };

            return View(resultadoPesquisa);
        }

    }
}
