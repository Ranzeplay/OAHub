using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.WorkflowModels
{
    public class Step
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AssigneesId { get; set; }

        public List<string> GetAssigneesId()
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(AssigneesId);
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SetAssigneesId(List<string> vs)
        {
            AssigneesId = JsonSerializer.Serialize(vs);
        }

        public WorkStatus Status { get; set; }
    }
}
