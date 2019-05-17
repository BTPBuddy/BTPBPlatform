using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTPBCommon.Projects;

namespace BTPBPlatform.Models
{
    /// <summary>
    /// Represents a platform project View-model 
    /// </summary>
    public class ClientProject
    {
        /// <summary>
        /// The name of the project being created.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The password of the client being signed in.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Creates a platform project object in the database corresponding to the 
        /// form-supplied project credentials.
        /// </summary>
        /// <param name="clientId">The client ID corresponding to the currently signed-in user.</param>
        /// <returns>the DB project object</returns>
        public Project MakeProject(int clientId)
        {
            Project project = new Project(Title, Description);
            project.AttributeToClient(clientId);
            project.Save();
            return project;
        }
    }
}
