using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Organization.Models.ViewModels.Extensions
{
    public class NewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string WebSite { get; set; }

        [Required]
        public string OrganizationRootUri { get; set; }

        [Required]
        public string CreateDashboardUri { get; set; }
    }
}
