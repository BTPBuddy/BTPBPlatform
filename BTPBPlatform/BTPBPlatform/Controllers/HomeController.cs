using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BTPBPlatform.Models;

namespace BTPBPlatform.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Qui sommes-nous ?";

            return View();
        }

        public IActionResult Platform()
        {
            ViewData["Message"] = "BTPBuddy Platform";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Obtenir plus d'informations";

            return View();
        }

        public IActionResult Login()
        {
            ViewData["Message"] = "Enter your username, client ID, and password";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
