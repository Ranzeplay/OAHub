using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.OrganizationModels
{
    public class ApiMember
    {
        public string UserId { get; set; }

        public string Position { get; set; }

        public DateTime JoinAt { get; set; }

        public bool IsLocked { get; set; }
    }
}
