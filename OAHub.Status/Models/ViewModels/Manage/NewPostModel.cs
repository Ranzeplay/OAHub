using OAHub.Base.Models.StatusModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models.ViewModels.Manage
{
    public class NewPostModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool ShowOnHeader { get; set; }

        [Required]
        public PostColor PostColor { get; set; }
    }
}
