using OAHub.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.ViewModels.Auth
{
    public class AuthorizeConfirmModel : AuthorizeModel
    {
        public bool Confirm { get; set; }

        public string AppName { get; set; }
    }
}
