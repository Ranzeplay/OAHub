using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.SurveyModel.Forms.Questions
{
    public class MultiSelect : Body
    {
        public List<string> Selections { get; set; }
    }
}
