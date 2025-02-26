using Microsoft.AspNetCore.Mvc;
using SportMotos.Models;

namespace SportMotos.Controllers
{
    public class Foruns : Controller
    {
        private readonly AppDbContext _context;
        public Foruns(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Forum()
        {
            return View();
        }
    }
}
