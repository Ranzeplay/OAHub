using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.Options
{
    public class JwtOptions
    {
        public readonly string DefaultIssuer = "OAHub.Passport";

        public readonly DateTime NotBefore = DateTime.UtcNow;

        public readonly DateTime ExpireTime = DateTime.UtcNow.AddMinutes(3);
    }
}
