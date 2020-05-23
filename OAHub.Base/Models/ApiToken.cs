using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models
{
    public class ApiToken
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string TokenContent { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
