using OAHub.Base.Models.StatusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models.ViewModels.Manage
{
    public class NewPostModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool ShowOnHeader { get; set; }

        public PostColor PostColor { get; set; }
    }
}
