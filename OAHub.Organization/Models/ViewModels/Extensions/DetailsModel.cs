using OAHub.Base.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Organization.Models.ViewModels.Extensions
{
    public class DetailsModel
    {
        public Extension Extension { get; set; }

        public string AuthorName { get; set; }

        public List<KeyValuePair<string, string>> UserJoinedOrganizations { get; set; }
    }
}
