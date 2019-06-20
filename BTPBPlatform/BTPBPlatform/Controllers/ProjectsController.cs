using BTPBCommon.Clients;
using BTPBCommon.Projects;
using BTPBCommon.Exceptions;
using BTPBPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTPBPlatform.Controllers
{
    [AuthenticateFilter]
    public class ProjectsController : Controller
    {
        public IActionResult Index()
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

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateProject");
        }

        [HttpGet]
        public IActionResult Project(int projectId)
        {
            Project project = new Project(projectId);

            int nDocuments = project.Documents.Capacity;
            List<ProjectDocument> allDocuments = BTPBCommon.Platform.ProjectDocuments;
            List<ProjectDocument> projectDocuments = new List<ProjectDocument>(nDocuments);
            projectDocuments.AddRange(allDocuments.Where(d =>
                d.Project.Id == projectId));
            

            string docs = "document";
            string association = "associé";

            if (nDocuments != 1)
            {
                docs += "s";
                association += "s";
            }

            ViewData["Title"] = project.Title;
            ViewData["Documents"] = projectDocuments;
            ViewData["ProjectId"] = projectId;
            ViewData["Message"] = Convert.ToString(nDocuments) + " " + docs + 
                " " + association + " avec ce projet";

            return View("Project");
        }

        [HttpGet]
        public IActionResult UploadDocument(int projectId)
        {
            ViewData["Title"] = "Charger un document.";
            ViewData["ProjectId"] = projectId;
            return View("Upload");
        }

        [HttpPost]
        public IActionResult UploadDocument(ClientProjectDocument document)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    document.MakeProjectDocument();
                    return RedirectToAction("Project", "Projects", new { projectId = document.ProjectId });
                }
                else
                {
                    throw new DBWriteException("Votre fichier n'a pas pu être chargé.");
                }
            }
            catch (BTPBException)
            {
                return RedirectToAction("Index", "Projects");
            }
            
        }

        [HttpPost]
        public IActionResult Create(ClientProject project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    project.MakeProject(SessionUtils.GetSessionClientId(HttpContext.Session));
                    return RedirectToAction("Index", "Projects");
                }
                catch (DBWriteException)
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
            
        }
    }
}
