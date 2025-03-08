using Microsoft.AspNetCore.Mvc;

namespace SportMotos.Controllers
{
    public class ContactosController : Controller
    {
        [HttpGet]
        public IActionResult Contactos()
        {
            return View();
        }
    }
}
