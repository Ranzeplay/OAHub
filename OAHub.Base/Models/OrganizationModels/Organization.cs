using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.OrganizationModels
{
    public class Organization
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OfficialWebsite { get; set; }

        public DateTime CreateTime { get; set; }

        public string FounderId { get; set; }

        public string Members { get; set; }

        public List<string> GetMembers()
        {
            if (!string.IsNullOrEmpty(Members))
            {
                return JsonSerializer.Deserialize<List<string>>(Members);
            }

            return new List<string>();
        }

        public void SetMembers(List<string> members)
        {
            Members = JsonSerializer.Serialize(members);
        }
    }
}
