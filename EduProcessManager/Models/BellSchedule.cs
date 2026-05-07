using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Models
{
    public class BellSchedule : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
