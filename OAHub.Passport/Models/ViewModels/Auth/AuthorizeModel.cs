using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.ViewModels.Auth
{
    public class AuthorizeModel
    {
        public string AppId { get; set; }

        public string RedirectUri { get; set; }

        public string State { get; set; }
    }
}
