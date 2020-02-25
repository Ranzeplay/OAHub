using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.WorkflowModels
{
    public class WorkflowOrganization
    {
        public string Id { get; set; }

        public string Secret { get; set; }

        public string ProjectsId { get; set; }

        public List<string> GetProjectsId()
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(ProjectsId);
            }
            catch
            {
                return new List<string>(); 
            }
        }

        public void SetProjectsId(List<string> vs)
        {
            ProjectsId = JsonSerializer.Serialize(vs);
        }
    }
}
