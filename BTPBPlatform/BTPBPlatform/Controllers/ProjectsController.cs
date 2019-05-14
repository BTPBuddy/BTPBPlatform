using BTPBCommon.Clients;
using BTPBPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTPBPlatform.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            if (SessionUtils.SessionAuthenticated(HttpContext.Session))
            {
                Client client = new Client(SessionUtils.FromJson(HttpContext.Session).ClientId);
                int nProjects = client.Projects.Capacity;
                string message = "";
                
                if (nProjects == 0)
                {
                    message = "Vous n'avez aucun projet en cours.";
                }
                else
                {
                    message = "Vous avez " + nProjects + " projets en cours.";
                }
                ViewData["Message"] = message;
                return View("Projects");
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
        }

        public IActionResult Create()
        {
            return View("CreateProject");
        }
    }
}
