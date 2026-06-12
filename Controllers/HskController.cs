using Microsoft.AspNetCore.Mvc;

namespace LingoToneMVC.Controllers
{
    public class HskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
