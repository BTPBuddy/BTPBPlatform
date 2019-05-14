using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BTPBPlatform.Models;
using BTPBCommon.Clients;

namespace BTPBPlatform.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            if (SessionUtils.SessionAuthenticated(HttpContext.Session))
            {
                ViewData["Message"] = "Votre espace BTPBuddy";

                ClientUser user = SessionUtils.FromJson(HttpContext.Session);
                Client client = new Client(user.ClientId);
                int nUsers = client.Accounts.Capacity;
                int nProjects = client.Projects.Capacity;

                ViewData["nUsers"] = nUsers;
                ViewData["nProjects"] = nProjects;

                return View("Account");
            }
            else
            {
                ViewData["Message"] = "Créer un projet";
                return RedirectToAction("Logout", "Login");
            }
        }
    }
}