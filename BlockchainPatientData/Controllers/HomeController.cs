using Microsoft.AspNetCore.Mvc;
using BlockchainPatientData.Models;

namespace BlockchainPatientData.Controllers
{
    /// <summary>
    /// Home Controller - Landing page and general navigation
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "HMS Blockchain - Home";
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "About Our System";
            return View();
        }

        public IActionResult Features()
        {
            ViewBag.Title = "System Features";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
