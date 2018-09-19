using Microsoft.AspNetCore.Mvc;

namespace Websock.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Home/About")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("Home/Contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Route("Home/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

