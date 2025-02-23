using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SportMotos.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace SportMotos.Controllers
{
    public class RecuperarContaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public RecuperarContaController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarLinkRecuperacao(string Email)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == Email);

            if (cliente == null)
            {
                ViewBag.Mensagem = "O Email não está cadastrado!";
                return View("RecuperarSenha"); // Retorna a mesma página com a mensagem de erro.
            }

            // Verifica se já existe uma entrada para este cliente
            var resetEntry = _context.PasswordResets.FirstOrDefault(r => r.IDCliente == cliente.IdCliente);

            string token = Guid.NewGuid().ToString();

            if (resetEntry == null)
            {
                resetEntry = new PasswordResets
                {
                    IDCliente = cliente.IdCliente,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1)
                };
                _context.PasswordResets.Add(resetEntry);
            }
            else
            {
                resetEntry.Token = token;
                resetEntry.Expiration = DateTime.UtcNow.AddHours(1);
                _context.PasswordResets.Update(resetEntry);
            }

            _context.SaveChanges(); // Salva as alterações no banco

            // Criar o link de redefinição de senha
            string? resetLink = Url.Action("RedefinirSenha", "RecuperarConta", new { token = token }, Request.Scheme);
            if (string.IsNullOrEmpty(resetLink))
            {
                throw new Exception("Erro ao gerar o link de redefinição de senha.");
            }

            // Enviar e-mail com o link
            EnviarEmail(cliente.Email, "Redefinição de Senha",
                $"Clique no link para redefinir sua senha: <a href='{resetLink}'>Redefinir Senha</a>");

            ViewBag.Mensagem = "Se este e-mail estiver cadastrado, enviamos um link para redefinir a senha.";
            return View("RecuperarSenhaConfirmacao"); // Retorna a página de confirmação
        }

        [HttpGet]
        public IActionResult RedefinirSenha(string token)
        {
            // Verifica se o token existe e não expirou
            var resetEntry = _context.PasswordResets.FirstOrDefault(r => r.Token == token && r.Expiration > DateTime.UtcNow);

            if (resetEntry == null)
            {
                ViewBag.Mensagem = "Token inválido ou expirado.";
                return RedirectToAction("RecuperarSenha"); // Retorna uma view informando que o token não é válido
            }

            // Passa o token para a view
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult RedefinirSenha(string token, string NovaSenha, string ConfirmarSenha)
        {
            // Verifica se as senhas coincidem
            if (NovaSenha != ConfirmarSenha)
            {
                ViewBag.Mensagem = "As senhas não coincidem.";
                ViewBag.Token = token;
                return View();
            }

            // Verifica se o token é válido
            var resetEntry = _context.PasswordResets.FirstOrDefault(r => r.Token == token && r.Expiration > DateTime.UtcNow);

            if (resetEntry == null)
            {
                ViewBag.Mensagem = "Token inválido ou expirado.";
                return View("ErroToken");
            }

            // Busca o cliente e redefine a senha
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == resetEntry.IDCliente);
            if (cliente == null || string.IsNullOrEmpty(cliente.Email))
            {
                TempData["Erro"] = "Erro ao processar a solicitação. Tente novamente.";
                return RedirectToAction("RecuperarSenha");
            }

            cliente.Password = NovaSenha; // Aqui você pode fazer um hash da senha antes de salvar
            _context.Clientes.Update(cliente);
            _context.PasswordResets.Remove(resetEntry); // Remove o token após o uso
            _context.SaveChanges();

            ViewBag.Mensagem = "Senha redefinida com sucesso! Agora podes fazer login.";
            return View("Login");
        }

        private void EnviarEmail(string destinatario, string assunto, string mensagem)
        {
            try
            {
                string? email = _config["SmtpSettings:Email"];
                string? senha = _config["SmtpSettings:Senha"];
                string? host = _config["SmtpSettings:Host"];
                string? portaStr = _config["SmtpSettings:Porta"];

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portaStr))
                {
                    throw new Exception("Configurações de SMTP ausentes ou inválidas.");
                }

                if (!int.TryParse(portaStr, out int porta))
                {
                    throw new Exception("A porta SMTP não é um número válido.");
                }

                SmtpClient client = new SmtpClient(host)
                {
                    Port = porta,
                    Credentials = new NetworkCredential(email, senha),
                    EnableSsl = true
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = assunto,
                    Body = mensagem,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(destinatario);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar e-mail: " + ex.Message);
            }
        }



        //                          **Código Errado (Ignorar)**
        // [HttpPost]
        // public IActionResult EnviarEmailRecuperação(string email)
        // {
        //     var user=_context.Clientes.FirstOrDefault(u=>u.Email==email);
        //     if (user == null) 
        //     {
        //         ViewBag.Error = "Email não encontrado!";
        //         return View("RecuperarSenha");
        //     }

        //     //Token
        //     string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        //     user.ResetToken = token;
        //     user.ResetTokenExpiration= DateTime.UtcNow.AddHours(1);

        //     _context.SaveChanges();

        //     string link = Url.Action("RedefenirSenha", "RecuperarConta", new { token = token, email = user.Email }, Request.Scheme);
        //     EnviarEmail(user.Email, "Rcuperação da Senha", "$Clique aqui para redefinir a sua senha; {link}")

        //     ViewBag.Message="Email de recuperação enviado.";
        //     return View();
        // }

        // [HttpGet]
        // public IActionResult RedefenirSenha(string token, string email)
        // {
        //     var user=_context.Cliente.FirstOrDefault(u=>u.Email==email&&u.ResetToken==token);

        //     if(user==null || user.ResetTokenExpiration<DateTime.UtcNow)
        //     {
        //         ViewBag.Error="Token inválido";
        //         return View("RecuperarSenha");
        //     }

        //     return View(new RedefinirSenhaViewModel { Token = token, UserId = resetEntry.UserId });
        // }
    }
}
