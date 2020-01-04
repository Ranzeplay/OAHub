using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.ViewModels.Auth
{
    public class CodeModel
    {
        public DateTime ExpireTime { get; set; }

        public string ForUserId { get; set; }
    }
}
