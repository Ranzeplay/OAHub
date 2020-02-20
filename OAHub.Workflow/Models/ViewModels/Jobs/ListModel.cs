using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models.ViewModels.Jobs
{
    public class ListModel
    {
        public string OrganizationName { get; set; }

        public string ProjectName { get; set; }

        public List<JobOverviewModel> Jobs { get; set; }
    }
}
