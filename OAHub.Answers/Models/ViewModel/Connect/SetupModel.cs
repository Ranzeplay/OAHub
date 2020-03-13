using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Answers.Models.ViewModels.Connect
{
    public class SetupModel
    {
        public string OrganizationId { get; set; }

        public string OrganizationSecret { get; set; }

        public string OrganizationName { get; set; }

        public bool ConfirmAction { get; set; }
    }
}
