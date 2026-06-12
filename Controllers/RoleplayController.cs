using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingoToneMVC.Models;
using System.Threading.Tasks;

namespace LingoToneMVC.Controllers
{
    public class RoleplayController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleplayController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserDisplayName = user.DisplayName;
                ViewBag.UserXP = user.XP;
                ViewBag.UserLevel = user.Level;
                ViewBag.UserStreak = user.Streak;
            }
            else
            {
                ViewBag.UserDisplayName = "Khách";
                ViewBag.UserXP = 0;
                ViewBag.UserLevel = 1;
                ViewBag.UserStreak = 0;
            }

            return View();
        }
    }
}
