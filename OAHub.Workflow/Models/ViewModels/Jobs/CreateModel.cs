using OAHub.Base.Models.WorkflowModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Models.ViewModels.Jobs
{
    public class CreateJobModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public WorkStatus Status { get; set; }

        [Required]
        public string ManagerId { get; set; }

        public List<SelectMemberModel> MembersAvailable { get; set; }
    }
}
