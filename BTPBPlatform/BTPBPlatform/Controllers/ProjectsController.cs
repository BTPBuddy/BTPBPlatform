using BTPBCommon.Clients;
using BTPBCommon.Projects;
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

                List<Project> clientProjects = new List<Project>(nProjects);

                string message = "";
                
                if (nProjects == 0)
                {
                    message = "Vous n'avez aucun projet en cours.";
                }
                else
                {
                    message = "Vous avez " + nProjects + " projets en cours.";
                    List<Project> allProjects = BTPBCommon.Platform.Projects;
                    clientProjects.AddRange(allProjects.Where(p =>
                        p.Owner.Id == SessionUtils.GetSessionClientId(HttpContext.Session)));
                }

                ViewData["Projects"] = clientProjects;
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
            if (SessionUtils.SessionAuthenticated(HttpContext.Session))
            {
                return View("CreateProject");
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            
        }

        [HttpPost]
        public IActionResult OnProjectCreate(ClientProject project)
        {
            if (ModelState.IsValid && SessionUtils.SessionAuthenticated(HttpContext.Session))
            {
                try
                {
                    project.MakeProject(SessionUtils.GetSessionClientId(HttpContext.Session));
                    return RedirectToAction("Index", "Projects");
                }
                catch (BTPBCommon.Exceptions.DBWriteException)
                {
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            
        }

        
    }
}
