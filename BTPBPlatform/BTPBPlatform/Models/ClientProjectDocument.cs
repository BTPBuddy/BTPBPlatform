using System;
using System.Collections.Generic;
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

        public string Description { get; set; }

        public byte[] FileContents { get; set; }

        public IFormFile File { get; set; }

        public int ProjectId { get; set; }
        
        public string TagsString { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public int ContentTypeId { get; } = 1;

        public string TypeTitle { get; set; }

        private string TypeKey
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

        private void PopulateTags()
        {
            if (TagsString.Length > 0)
            {
                string[] splits = TagsString.Split(new string[] { ", " }, StringSplitOptions.None);
                if (splits.Length > 0)
                {
                    foreach (string tag in splits)
                    {
                        Tags.Add(tag);
                    }
                }
                else
                {
                    Tags.Add(TagsString);
                }
            }
        }

        private async Task ReadFileContents()
        {
            using (FileStream fs = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                await File.CopyToAsync(fs);
                using (MemoryStream ms = new MemoryStream())
                {
                    await fs.CopyToAsync(ms);
                    FileContents = ms.ToArray();
                }
            }
        }

        public async void MakeProjectDocument()
        {
            Title = File.FileName;

            PopulateTags();
            await ReadFileContents();

            ProjectDocument document = new ProjectDocument(ProjectId, Title, TypeKey, Description, Tags);
            document.Save();

            ProjectDocumentContent content = new ProjectDocumentContent(document.Id, Title, FileContents, ContentTypeId);
            content.Save();
        }
    }
}
