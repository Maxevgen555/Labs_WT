using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labs.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
