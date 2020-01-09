using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Models.ViewModels.Apps
{
    public class NewAppModel
    {
        [Required]
        public string Name { get; set; }
    }
}
