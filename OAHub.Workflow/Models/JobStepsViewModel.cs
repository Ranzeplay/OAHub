using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models
{
    public class JobStepsViewModel : JobOverviewModel
    {
        public List<StepViewModel> Steps { get; set; }
    }
}
