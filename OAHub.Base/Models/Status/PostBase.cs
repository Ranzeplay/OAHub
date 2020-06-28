using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.Status
{
    public class PostBase
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public PostColor Color { get; set; }

        public DateTime PublishTime { get; set; }

        public bool ShowOnHeader { get; set; }
    }
}
