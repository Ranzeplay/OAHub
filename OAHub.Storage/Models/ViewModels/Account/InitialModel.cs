using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Account
{
    public class InitialModel
    {
        [Required]
        public string ShelfName { get; set; }

        [Required]
        public string ShelfDescrption { get; set; }
    }
}
