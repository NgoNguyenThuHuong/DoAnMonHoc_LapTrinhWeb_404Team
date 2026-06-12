using Microsoft.AspNetCore.Mvc;

namespace LingoToneMVC.Controllers
{
    public class PronunciationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
