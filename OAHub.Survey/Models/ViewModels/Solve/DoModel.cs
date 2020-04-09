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

        public List<KeyValuePair<Base.Models.SurveyModel.Forms.Questions.Type, object>> Questions { get; set; }
    }
}
