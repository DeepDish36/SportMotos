using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMotos.Models;
using System.Security.Claims;

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
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            Console.WriteLine($"Recebido no método Adicionar: ID_Anuncio = {request?.Id_Anuncio}, TipoAnuncio = {request?.TipoAnuncio}");

            if (request == null || string.IsNullOrEmpty(request.TipoAnuncio))
            {
                Console.WriteLine("Adicionar: Requisição inválida ou Tipo de anúncio está vazio.");
                return BadRequest(new { sucesso = false, mensagem = "Requisição ou tipo de anúncio inválido." });
            }

            if (request.Id_Anuncio <= 0)
            {
                Console.WriteLine("Adicionar: ID do anúncio inválido.");
                return BadRequest(new { sucesso = false, mensagem = "ID do anúncio deve ser maior que 0." });
            }

            var idClienteClaim = User.FindFirst("IdCliente")?.Value;
            if (string.IsNullOrEmpty(idClienteClaim) || !int.TryParse(idClienteClaim, out var idCliente))
            {
                Console.WriteLine("Adicionar: Utilizador não autenticado ou IdCliente inválido.");
                return Unauthorized(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            // Validar e ajustar o ID do anúncio com base no tipo de anúncio
            if (request.TipoAnuncio == "motos")
            {
                var anuncio = await _context.AnuncioMotos
                    .FirstOrDefaultAsync(a => a.IdMotoNavigation.IdMoto == request.Id_Anuncio);

                if (anuncio == null)
                {
                    Console.WriteLine("Anúncio de moto não encontrado.");
                    return BadRequest(new { sucesso = false, mensagem = "Anúncio de moto inválido." });
                }

                request.Id_Anuncio = anuncio.IdAnuncioMoto; // Atualiza o ID para o ID do anúncio
                Console.WriteLine($"ID atualizado para anúncio de moto: {request.Id_Anuncio}");
            }
            else if (request.TipoAnuncio == "pecas")
            {
                var anuncio = await _context.AnuncioPecas
                    .FirstOrDefaultAsync(a => a.IdPecaNavigation.IdPeca == request.Id_Anuncio);

                if (anuncio == null)
                {
                    Console.WriteLine("Anúncio de peça não encontrado.");
                    return BadRequest(new { sucesso = false, mensagem = "Anúncio de peça inválido." });
                }

                request.Id_Anuncio = anuncio.IdAnuncioPeca; // Atualiza o ID para o ID do anúncio
                Console.WriteLine($"ID atualizado para anúncio de peça: {request.Id_Anuncio}");
            }
            else
            {
                Console.WriteLine("Adicionar: Tipo de anúncio inválido.");
                return BadRequest(new { sucesso = false, mensagem = "Tipo de anúncio inválido." });
            }

            Console.WriteLine($"Verificando duplicação para Cliente: {idCliente}, Anúncio: {request.Id_Anuncio}, Tipo: {request.TipoAnuncio}");

            var favoritoExistente = await _context.Favoritos
                .AnyAsync(f => f.Id_Cliente == idCliente && f.Id_Anuncio == request.Id_Anuncio && f.TipoAnuncio == request.TipoAnuncio);

            if (favoritoExistente)
            {
                Console.WriteLine("Adicionar: Favorito já existe.");
                return BadRequest(new { sucesso = false, mensagem = "O favorito já existe." });
            }

            try
            {
                var favorito = new Favoritos
                {
                    Id_Cliente = idCliente,
                    Id_Anuncio = request.Id_Anuncio,
                    TipoAnuncio = request.TipoAnuncio,
                    DataAdicionado = DateTime.Now
                };

                Console.WriteLine($"Salvando favorito para Cliente: {idCliente}, Anúncio: {request.Id_Anuncio}, Tipo: {request.TipoAnuncio}");
                _context.Favoritos.Add(favorito);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    dados = new
                    {
                        IdFavorito = favorito.Id,
                        favorito.Id_Anuncio,
                        favorito.TipoAnuncio,
                        favorito.DataAdicionado
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar favorito: {ex.Message}");
                return StatusCode(500, new { sucesso = false, mensagem = "Erro interno ao processar o pedido." });
            }
        }

        [HttpPost("Remover")]
        public async Task<IActionResult> Remover([FromBody] FavoritoRequest request)
        {
            Console.WriteLine($"Recebido no método Remover: ID_Anuncio = {request?.Id_Anuncio}, TipoAnuncio = {request?.TipoAnuncio}");

            // Validação inicial
            if (request == null || string.IsNullOrEmpty(request.TipoAnuncio))
            {
                Console.WriteLine("Remover: Requisição inválida ou Tipo de anúncio está vazio.");
                return BadRequest(new { sucesso = false, mensagem = "Requisição ou tipo de anúncio inválido." });
            }

            if (request.Id_Anuncio <= 0)
            {
                Console.WriteLine("Remover: ID do anúncio inválido.");
                return BadRequest(new { sucesso = false, mensagem = "ID do anúncio deve ser maior que 0." });
            }

            // Recuperar IdCliente dos claims
            var idClienteClaim = User.FindFirst("IdCliente")?.Value;
            if (string.IsNullOrEmpty(idClienteClaim) || !int.TryParse(idClienteClaim, out var idCliente))
            {
                Console.WriteLine("Remover: Utilizador não autenticado ou IdCliente inválido.");
                return Unauthorized(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            // Validar e traduzir o IdMoto para IdAnuncioMoto ou IdAnuncioPeca
            if (request.TipoAnuncio == "motos")
            {
                var anuncio = await _context.AnuncioMotos
                    .FirstOrDefaultAsync(a => a.IdMotoNavigation.IdMoto == request.Id_Anuncio);

                if (anuncio == null)
                {
                    Console.WriteLine("Remover: Anúncio de moto não encontrado.");
                    return BadRequest(new { sucesso = false, mensagem = "Anúncio de moto inválido." });
                }

                request.Id_Anuncio = anuncio.IdAnuncioMoto; // Atualiza o ID para o ID do anúncio
                Console.WriteLine($"ID atualizado para anúncio de moto: {request.Id_Anuncio}");
            }
            else if (request.TipoAnuncio == "pecas")
            {
                var anuncio = await _context.AnuncioPecas
                    .FirstOrDefaultAsync(a => a.IdPecaNavigation.IdPeca == request.Id_Anuncio);

                if (anuncio == null)
                {
                    Console.WriteLine("Remover: Anúncio de peça não encontrado.");
                    return BadRequest(new { sucesso = false, mensagem = "Anúncio de peça inválido." });
                }

                request.Id_Anuncio = anuncio.IdAnuncioPeca; // Atualiza o ID para o ID do anúncio
                Console.WriteLine($"ID atualizado para anúncio de peça: {request.Id_Anuncio}");
            }
            else
            {
                Console.WriteLine("Remover: Tipo de anúncio inválido.");
                return BadRequest(new { sucesso = false, mensagem = "Tipo de anúncio inválido." });
            }

            Console.WriteLine($"Buscando favorito para remoção: Cliente = {idCliente}, Anúncio = {request.Id_Anuncio}, Tipo = {request.TipoAnuncio}");

            // Buscar e remover o favorito
            var favorito = await _context.Favoritos.FirstOrDefaultAsync(f =>
                f.Id_Cliente == idCliente && f.Id_Anuncio == request.Id_Anuncio && f.TipoAnuncio == request.TipoAnuncio);

            if (favorito == null)
            {
                Console.WriteLine("Remover: Favorito não encontrado.");
                return NotFound(new { sucesso = false, mensagem = "Favorito não encontrado." });
            }

            try
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Favorito removido: Cliente = {idCliente}, Anúncio = {request.Id_Anuncio}, Tipo = {request.TipoAnuncio}");
                return Ok(new { sucesso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover favorito: {ex.Message}");
                return StatusCode(500, new { sucesso = false, mensagem = "Erro interno ao processar o pedido." });
            }
        }

        [HttpGet("Verificar")]
        public async Task<IActionResult> Verificar(int idAnuncio, string tipoAnuncio)
        {
            var idClienteClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClienteClaim) || !int.TryParse(idClienteClaim, out var idCliente))
            {
                Console.WriteLine("Verificar: Utilizador não autenticado.");
                return Unauthorized(new { sucesso = false, mensagem = "Utilizador não autenticado." });
            }

            var favoritoExistente = await _context.Favoritos
                .AnyAsync(f => f.Id_Cliente == idCliente && f.Id_Anuncio == idAnuncio && f.TipoAnuncio == tipoAnuncio);

            return Ok(new { sucesso = true, isFavorito = favoritoExistente });
        }

        [Authorize]
        public async Task<IActionResult> MeusAnuncios()
        {
            // Verifica se o usuário é um cliente
            var idClienteClaim = User.FindFirst("IdCliente")?.Value;
            int? idCliente = null;
            if (!string.IsNullOrEmpty(idClienteClaim) && int.TryParse(idClienteClaim, out var parsedIdCliente))
            {
                idCliente = parsedIdCliente;
            }

            // Verifica se o usuário é um admin
            var idAdminClaim = User.FindFirst("IdAdmin")?.Value;
            int? idAdmin = null;
            if (!string.IsNullOrEmpty(idAdminClaim) && int.TryParse(idAdminClaim, out var parsedIdAdmin))
            {
                idAdmin = parsedIdAdmin;
            }

            // Se nenhum ID for encontrado, retorna não autorizado
            if (idCliente == null && idAdmin == null)
            {
                return Unauthorized();
            }

            // Busca os favoritos com base no ID do cliente ou do admin
            var favoritos = await _context.Favoritos
                .Include(f => f.AnuncioMoto) // Carrega os dados de motos quando aplicável
                .Include(f => f.AnuncioPeca) // Carrega os dados de peças quando aplicável
                .Where(f => (idCliente != null && f.Id_Cliente == idCliente) || (idAdmin != null && f.Id_Cliente == idAdmin))
                .ToListAsync();

            return View(favoritos);
        }
    }

    public class FavoritoRequest
    {
        public int Id_Anuncio { get; set; }
        public string TipoAnuncio { get; set; } = null!;
    }
}