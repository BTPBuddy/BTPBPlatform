using BTPBCommon.Clients;
using BTPBCommon.Projects;
using BTPBCommon.Exceptions;
using BTPBCommon.Codes;
using BTPBPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BTPBPlatform.Controllers
{
    [AuthenticateFilter]
    public class ProjectsController : Controller
    {
        #region Actions
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

        [HttpPost]
        public IActionResult DeleteProject(int projectId)
        {
            Project project = new Project(projectId);
            project.Remove();
            return RedirectToAction("Index", "Projects");
        }

        [HttpPost]
        public IActionResult DeleteDocument(int documentId)
        {
            ProjectDocument document = new ProjectDocument(documentId);
            int pId = document.Project.Id;
            document.Remove();
            return RedirectToAction("Project", "Projects", new { projectId = pId });
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
                    MakeProjectDocument(document);
                    return RedirectToAction("Project", "Projects", new { projectId = document.ProjectId });
                }
                else
                {
                    throw new DBWriteException("Votre fichier n'a pas pu être chargé.");
                }
            }
            catch (BTPBException ex)
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

        #endregion

        #region Helper Methods

        protected void PopulateTags(ClientProjectDocument document)
        {
            if (document.TagsString.Length > 0)
            {
                string[] splits = document.TagsString.Split(new string[] { ", " }, StringSplitOptions.None);
                foreach (string tag in splits)
                {
                    document.Tags.Add(tag);
                }
            }
        }

        protected void ReadFileContents(ClientProjectDocument document)
        {
            using (FileStream fs = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                document.File.CopyTo(fs);
                document.FileContents = new byte[fs.Length];
                fs.Read(document.FileContents, 0, (int)fs.Length);
            }
        }

        protected void MakeProjectDocument(ClientProjectDocument document)
        {
            PopulateTags(document);
            //ReadFileContents(document);
            document.FileContents = UploadFile(document.File);
            ProjectDocument _document = new ProjectDocument(document.ProjectId, document.Title, document.TypeKey, document.Description, document.Tags);
            _document.Save();

            ProjectDocumentContentTypeCode _contentType = null;
            try
            {
                 _contentType = new ProjectDocumentContentTypeCode(document.File.ContentType);
            }
            catch
            {
                _contentType = new ProjectDocumentContentTypeCode() {
                    Title = document.File.ContentType,
                    Key = document.File.ContentType,
                    Active = true,
                    Order = 0
                };
                _contentType.Save();
            }
            ProjectDocumentContent content = new ProjectDocumentContent(_document.Id, document.File.FileName, document.FileContents, _contentType.Id);
            content.Save();

            foreach (string tag in document.Tags)
            {
                ProjectDocumentTag oTag = new ProjectDocumentTag(_document.Id, tag);
                try
                {
                    oTag.Save();
                }
                catch (BTPBCommon.Exceptions.BTPBException ex)
                {
                    throw ex;
                }
            }
        }



        public byte[] UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Empty File.");
            }

            byte[] content;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                content = new byte[memoryStream.Length];
                memoryStream.Read(content, 0, content.Length);
            }
            return content;
        }
/*
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        */
        #endregion

    }
}
