using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models.ViewModels.Projects
{
    public class OverviewModel
    {
        public string OrganizationName { get; set; }
        public List<ProjectViewModel> Projects { get; set; }
    }
}
