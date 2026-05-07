using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Services
{
    public class EventStatusService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
