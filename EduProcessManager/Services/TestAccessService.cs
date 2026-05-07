using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Services
{
    public class TestAccessService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
