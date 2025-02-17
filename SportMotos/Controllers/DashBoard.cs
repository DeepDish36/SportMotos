using Microsoft.AspNetCore.Mvc;

namespace SportMotos.Controllers
{
    public class DashBoard : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
