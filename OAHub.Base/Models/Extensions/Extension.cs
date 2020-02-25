using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.Extensions
{
    public class Extension
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string WebSite { get; set; }

        public string OrganizationRootUri { get; set; }

        public string CreateDashboardUri { get; set; }

        public string AuthorId { get; set; }
    }
}
