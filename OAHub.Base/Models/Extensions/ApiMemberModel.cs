using OAHub.Base.Models.OrganizationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Base.Models.Extensions
{
    public class ApiMemberModel
    {
        public string MemberId { get; set; }

        public string FullName { get; set; }

        public string Position { get; set; }

        public string Email { get; set; }
    }
}
