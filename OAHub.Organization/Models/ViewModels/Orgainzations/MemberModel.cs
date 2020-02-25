using OAHub.Base.Models.OrganizationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Organization.Models.ViewModels.Orgainzations
{
    public class MemberModel
    {
        public ApiMember Member { get; set; }

        public OrganizationUser User { get; set; }
    }
}
