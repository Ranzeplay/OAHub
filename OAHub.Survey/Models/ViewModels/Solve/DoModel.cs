using OAHub.Base.Models.SurveyModels.Forms.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Survey.Models.ViewModels.Solve
{
    public class DoModel
    {
        public string FormId { get; set; }

        public string Title { get; set; }

        public List<KeyValuePair<QuestionType, object>> Questions { get; set; }
    }
}
