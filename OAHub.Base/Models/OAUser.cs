using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models
{
    public class OAUser : IdentityUser
    {
        public override string Id { get => base.Id; set => base.Id = value; }

        public override string Email { get => base.Email; set => base.Email = value; }

        public override string UserName { get => base.UserName; set => base.UserName = value; }

        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        public DateTime CreateTime { get; set; }
    }
}
