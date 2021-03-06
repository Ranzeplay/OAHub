using OAHub.Base.Models.WorkflowModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models
{
    public class StepViewModel
    {
        public string NameEncoded { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<WorkflowUser> Assignees { get; set; }

        public long AssigneesCount { get; set; }

        public WorkStatus Status { get; set; }

    }
}