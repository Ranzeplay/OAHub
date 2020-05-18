using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.SurveyModels.Forms.Answers
{
    public class MultiSelect : Body
    {
        public List<int> SelectedIndexes { get; set; }
    }
}
