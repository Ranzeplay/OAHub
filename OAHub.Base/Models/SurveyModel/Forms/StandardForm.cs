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

        public List<KeyValuePair<Questions.Type, object>> GetContent()
        {
            try
            {
                return JsonSerializer.Deserialize<List<KeyValuePair<Questions.Type, object>>>(Content);
            }
            catch
            {
                return new List<KeyValuePair<Questions.Type, object>>();
            }
        }

        public void SetContent(List<KeyValuePair<Questions.Type, object>> kvp)
        {
            List<KeyValuePair<Questions.Type, object>> kvpFinal = new List<KeyValuePair<Questions.Type, object>>();
            foreach (var item in kvp)
            {
                kvpFinal.Add(new KeyValuePair<Questions.Type, object>(item.Key, item.Value));
            }

            Content = JsonSerializer.Serialize(kvpFinal);
        }

        public DateTime CreateTime { get; set; }

        public DateTime Deadline { get; set; }

        public string Submitters { get; set; }
    }
}
