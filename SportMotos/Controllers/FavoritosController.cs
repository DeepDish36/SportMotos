using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    [Route("Favoritos")]
    public class FavoritosController : Controller
    {
        private readonly AppDbContext _context;

        public FavoritosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Adicionar")]
        public async Task<IActionResult> Adicionar([FromBody] FavoritoRequest request)
        {
            if (request == null || request.AnuncioId <= 0 || string.IsNullOrEmpty(request.TipoAnuncio))
                return BadRequest(new { sucesso = false, mensagem = "Dados inválidos." });

            // Recuperar o IdCliente dos claims
            var idClienteClaim = User.FindFirst("IdCliente")?.Value;
            Console.WriteLine($"IdCliente recuperado dos claims: {idClienteClaim}");
            if (string.IsNullOrEmpty(idClienteClaim))
                return Unauthorized(new { sucesso = false, mensagem = "Utilizador não autenticado." });

            var idCliente = int.Parse(idClienteClaim);

            var favorito = new Favoritos
            {
                Id_Cliente = idCliente,
                AnuncioId = request.AnuncioId,
                TipoAnuncio = request.TipoAnuncio,
                DataAdicionado = DateTime.Now
            };

            _context.Favoritos.Add(favorito);
            await _context.SaveChangesAsync();

            return Ok(new { sucesso = true });
        }

        //[HttpPost("Remover")]
        //public async Task<IActionResult> Remover([FromBody] FavoritoRequest request)
        //{
        //    try
        //    {
        //        if (request == null || request.AnuncioId <= 0 || string.IsNullOrEmpty(request.TipoAnuncio))
        //            return BadRequest(new { sucesso = false, mensagem = "Dados inválidos." });

        //        var favorito = await _context.Favoritos
        //            .FirstOrDefaultAsync(f => f.AnuncioId == request.AnuncioId && f.TipoAnuncio == request.TipoAnuncio);

        //        if (favorito == null)
        //            return NotFound(new { sucesso = false, mensagem = "Favorito não encontrado." });

        //        _context.Favoritos.Remove(favorito);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { sucesso = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log do erro
        //        Console.WriteLine($"Erro ao remover favorito: {ex.Message}");
        //        return StatusCode(500, new { sucesso = false, mensagem = "Erro interno no servidor." });
        //    }
        //}
    }

    public class FavoritoRequest
    {
        public int AnuncioId { get; set; }
        public string TipoAnuncio { get; set; } = null!;
    }
}
