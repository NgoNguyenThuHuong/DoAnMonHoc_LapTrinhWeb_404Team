using Microsoft.AspNetCore.Mvc;

namespace LingoToneMVC.Controllers
{
    public class WritingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
