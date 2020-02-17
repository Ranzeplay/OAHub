using OAHub.Base.Models.WorkflowModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models.ViewModels.Projects
{
    public class DetailsModel
    {
        public ProjectViewModel Project { get; set; }

        public List<JobViewModel> Jobs { get; set; }
    }
}
