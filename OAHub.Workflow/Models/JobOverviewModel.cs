using OAHub.Base.Models.WorkflowModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models
{
    public class JobOverviewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public WorkflowUser Manager { get; set; }

        public WorkStatus Status { get; set; }

        public int StepsCount { get; set; }
    }
}
