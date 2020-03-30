using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Survey.Areas.CreateSurvey.Models.ViewModels.Standard
{
    public class SummaryModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public bool AllowAnonymous { get; set; }

        public bool AllowMultipleSubmits { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime Deadline { get; set; }

        public Dictionary<Base.Models.SurveyModel.Forms.Questions.Type, object> Content { get; set; }
    }
}
