using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BTPBCommon.Projects;
using Microsoft.AspNetCore.Http;

namespace BTPBPlatform.Models
{
    public class ClientProjectDocument
    {
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public byte[] FileContents { get; set; }

        public IFormFile File { get; set; }

        public int ProjectId { get; set; }
        
        public string TagsString { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public int ContentTypeId { get; } = 1;

        public string TypeTitle { get; set; }

        public string TypeKey
        {
            get
            {
                if (TypeTitle.Equals("Facture"))
                {
                    return "fact";
                }
                else if (TypeTitle.Equals("Juridique"))
                {
                    return "jur";
                }
                else
                {
                    return "doc";
                }
            }
        }

    }
}
