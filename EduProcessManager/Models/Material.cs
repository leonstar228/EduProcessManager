using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Models
{
    public class Material : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
