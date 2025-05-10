using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SportMotos.Models;
using SportMotos.Services;
using System.Security.Claims;

namespace SportMotos.Controllers
{
    public class InteresseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public InteresseController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService; // Inicializa o serviço de email
        }

        public async Task<IActionResult> MotosInteressadas()
        {
            var userIdClaim = User.FindFirst("IdCliente")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            int idCliente = int.Parse(userIdClaim);

            var motosInteressadas = await _context.InteresseMotos
                .Where(i => i.IdCliente == idCliente)
                .Select(i => i.Moto)
                .ToListAsync();

            return View(motosInteressadas);
        }

        [Authorize] // Garante que só usuários logados acessem
        public IActionResult Criar(int idMoto)
        {
            if (idMoto <= 0)
            {
                Console.WriteLine("ID_Moto inválido recebido: " + idMoto);
                return BadRequest("ID_Moto inválido.");
            }

            var model = new InteresseMotos { IdMoto = idMoto };
            Console.WriteLine($"Modal carregada com ID_Moto: {model.IdMoto}");
            return PartialView("_FormularioInteresse", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SalvarInteresse(InteresseMotos interesse)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return Json(new { success = false, message = "Dados inválidos. Verifique os campos obrigatórios." });
            }

            // Obter o tipo de utilizador logado
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            // Verificar se o usuário é um admin
            if (tipoUsuario == "Admin")
            {
                Console.WriteLine("Admins não podem fazer pedidos.");
                return Json(new { success = false, message = "Admins não podem fazer pedidos." });
            }

            // Obter o ID do cliente logado
            var userId = User.FindFirstValue("IdCliente");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idCliente))
            {
                Console.WriteLine("Erro ao obter ID do cliente.");
                return Json(new { success = false, message = "Erro ao identificar o cliente logado." });
            }

            Console.WriteLine($"IdCliente obtido: {idCliente}");

            // Verificar se o ID da moto existe na base de dados
            var motoExistente = await _context.Motos.AnyAsync(m => m.IdMoto == interesse.IdMoto);
            if (!motoExistente)
            {
                Console.WriteLine($"A moto com ID_Moto = {interesse.IdMoto} não existe na base de dados.");
                return Json(new { success = false, message = "A moto selecionada não existe." });
            }

            Console.WriteLine($"Moto com ID_Moto = {interesse.IdMoto} foi validada.");

            try
            {
                // Configuração para salvar interesse
                interesse.IdCliente = idCliente;
                interesse.DataInteresse = DateTime.Now;

                _context.InteresseMotos.Add(interesse);
                await _context.SaveChangesAsync();

                Console.WriteLine("Interesse salvo com sucesso!");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar interesse: {ex.Message}");
                return Json(new { success = false, message = "Erro ao salvar no banco de dados. Tente novamente mais tarde." });
            }
        }

        [Authorize]
        public async Task<IActionResult> AprovarPedido(int id, string email)
        {
            var tipoUsuario = User.FindFirstValue("Tipo_Utilizador");

            if (tipoUsuario != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var interesse = await _context.InteresseMotos.FindAsync(id);
            if (interesse == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            // Atualiza o status do pedido para "Aprovado"
            interesse.Status = "Aprovado";
            _context.Update(interesse);
            await _context.SaveChangesAsync();

            // Envia o email ao cliente
            await EnviarEmailAprovacao(email, interesse);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RejeitarPedido(int id)
        {
            var interesse = await _context.InteresseMotos.FindAsync(id);
            if (interesse == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            // Atualiza o status do pedido para "Rejeitado"
            interesse.Status = "Rejeitado";
            _context.Update(interesse);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task EnviarEmailAprovacao(string email, InteresseMotos interesse)
        {
            // Carregar o cliente e a moto relacionados ao interesse
            var interesseCompleto = await _context.InteresseMotos
                .Include(i => i.Cliente)
                .Include(i => i.Moto)
                .FirstOrDefaultAsync(i => i.IdInteresse == interesse.IdInteresse);

            if (interesseCompleto == null || interesseCompleto.Cliente == null || interesseCompleto.Moto == null)
            {
                Console.WriteLine("Erro ao carregar os dados do interesse, cliente ou moto.");
                return;
            }

            var nomeCliente = interesseCompleto.Cliente.Nome;
            var marcaMoto = interesseCompleto.Moto.Marca;
            var modeloMoto = interesseCompleto.Moto.Modelo;

            var subject = "Pedido Aprovado";
            var message = $@"
                <html>
                <body>
                    <h2>Olá, {nomeCliente}!</h2>
                    <p>Temos o prazer de informar que o seu pedido para a moto <strong>{marcaMoto} {modeloMoto}</strong> foi aprovado com sucesso.</p>
                    <p>Entre em contato conosco para mais informações ou para agendar a retirada.</p>
                    <h3>Horário de funcionamento:</h3>
                    <ul>
                        <li><strong>Seg. a Sex.:</strong> 9:00 às 18:00</li>
                        <li><strong>Sáb.:</strong> 9:00 às 17:00</li>
                        <li><strong>Dom.:</strong> Encerrados</li>
                    </ul>
                    <h3>Contactos:</h3>
                    <ul>
                        <li><strong>Tlm:</strong> 922333444</li>
                        <li><strong>Tlf:</strong> 232876554</li>
                        <li><strong>Email:</strong> sportmotos@gmail.com</li>
                    </ul>
                    <p>Estamos à sua disposição para quaisquer dúvidas ou suporte adicional.</p>
                    <p>Atenciosamente,</p>
                    <p><strong>SportMotos</strong></p>
                </body>
                </html>
            ";
            // Enviar o email
            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}
