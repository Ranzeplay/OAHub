using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.SurveyModels.Forms.Questions
{
    public class MultiSelect : Body
    {
        public List<string> Selections { get; set; }
    }
}
