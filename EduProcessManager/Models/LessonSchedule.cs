using Microsoft.AspNetCore.Mvc;

namespace EduProcessManager.Models
{
    public class LessonSchedule : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
