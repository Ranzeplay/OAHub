using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.SurveyModel.Forms
{
    public class StandardForm
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string AuthorId { get; set; }

        public bool AllowAnonymous { get; set; }

        public bool AllowMultipleSubmits { get; set; }

        // Will be save as <Type, Details>
        public string Content { get; set; }

        public Dictionary<Questions.Type, object> GetContent()
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<Questions.Type, object>>(Content);
            }
            catch
            {
                return new Dictionary<Questions.Type, object>();
            }
        }

        public void SetContent(Dictionary<Questions.Type, object> kvp)
        {
            Content = JsonSerializer.Serialize(kvp);
        }

        public DateTime CreateTime { get; set; }

        public DateTime Deadline { get; set; } 

        public string Submitters { get; set; }
    }
}
