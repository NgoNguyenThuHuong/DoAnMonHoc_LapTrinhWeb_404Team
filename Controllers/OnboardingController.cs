using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LingoToneMVC.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
