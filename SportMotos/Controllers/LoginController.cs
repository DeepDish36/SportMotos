using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class LoginController:Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
