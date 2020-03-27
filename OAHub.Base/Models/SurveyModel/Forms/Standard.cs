using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.SurveyModel.Forms
{
    public class Standard
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string AuthorId { get; set; }

        public bool AllowAnonymous { get; set; }

        public bool AllowMultipleSubmits { get; set; }

        // Will be save as <Type, Details>
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime Deadline { get; set; } 

        public string Submitts { get; set; }
    }
}
