using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Services
{
    public class TestStatusService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
