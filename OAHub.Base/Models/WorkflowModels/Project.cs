using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.WorkflowModels
{
    public class ProjectViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ManagerId { get; set; }

        public DateTime CreateTime { get; set; }

        public WorkStatus Status { get; set; }

        public string JobsId { get; set; }

        public void SetJobsId(List<string> vs)
        {
            JobsId = JsonSerializer.Serialize(vs);
        }

        public List<string> GetJobsId()
        {
            if (!string.IsNullOrEmpty(JobsId))
            {
                return JsonSerializer.Deserialize<List<string>>(JobsId);
            }

            return new List<string>();
        }
    }
}
