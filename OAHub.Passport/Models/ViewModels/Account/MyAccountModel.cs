using OAHub.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.ViewModels.Account
{
    public class MyAccountModel
    {
        public OAUser User { get; set; }

        public List<App> AuthorizedApps { get; set; }

        public List<App> AppsCreatedByUser { get; set; }
    }
}
