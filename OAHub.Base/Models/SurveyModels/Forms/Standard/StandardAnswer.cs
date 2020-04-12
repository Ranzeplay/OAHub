using OAHub.Base.Models.SurveyModels.Forms.Questions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.SurveyModels.Forms.Standard
{
    public class StandardAnswer
    {
        public string Id { get; set; }

        public string ForFormId { get; set; }

        public string Submitter { get; set; }

        // Will be save as <Type, Details>
        public string Content { get; set; }

        public List<KeyValuePair<QuestionType, object>> GetContent()
        {
            try
            {
                return JsonSerializer.Deserialize<List<KeyValuePair<QuestionType, object>>>(Content);
            }
            catch
            {
                return new List<KeyValuePair<QuestionType, object>>();
            }
        }

        public void SetContent(List<KeyValuePair<QuestionType, object>> kvp)
        {
            List<KeyValuePair<QuestionType, object>> kvpFinal = new List<KeyValuePair<QuestionType, object>>();
            foreach (var item in kvp)
            {
                kvpFinal.Add(new KeyValuePair<QuestionType, object>(item.Key, item.Value));
            }

            Content = JsonSerializer.Serialize(kvpFinal);
        }

        public DateTime SubmitTime { get; set; }
    }
}
