using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.WorkflowModels
{
    public class Job
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ManagerId { get; set; }

        public WorkStatus Status { get; set; }

        public string Steps { get; set; }

        public void GetSteps(List<Step> steps)
        {
            Steps = JsonSerializer.Serialize(steps);
        }

        public List<Step> GetSteps()
        {
            if (!string.IsNullOrEmpty(Steps))
            {
                return JsonSerializer.Deserialize<List<Step>>(Steps);
            }

            return new List<Step>();
        }
    }
}
