using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTPBPlatform.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Votre compte";
            return View("Account");
        }
    }
}