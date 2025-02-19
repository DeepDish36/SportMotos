using Microsoft.AspNetCore.Mvc;

namespace SportMotos.Controllers
{
    public class Contactos : Controller
    {
        public IActionResult MostrarContactos()
        {
            return View();
        }
    }
}
