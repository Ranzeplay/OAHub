using OAHub.Base.Models.Extensions;
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

        public List<Member> GetMembers()
        {
            if (!string.IsNullOrEmpty(Members))
            {
                return JsonSerializer.Deserialize<List<Member>>(Members);
            }

            return new List<Member>();
        }

        public void SetMembers(List<Member> members)
        {
            Members = JsonSerializer.Serialize(members);
        }

        public string ExtensionsInstalled { get; set; }

        public List<ExtensionCredential> GetExtensionsInstalled()
        {
            if (!string.IsNullOrEmpty(ExtensionsInstalled))
            {
                return JsonSerializer.Deserialize<List<ExtensionCredential>>(ExtensionsInstalled);
            }

            return new List<ExtensionCredential>();
        }

        public void SetExtensionsInstalled(List<ExtensionCredential> extensionsInstalled)
        {
            ExtensionsInstalled = JsonSerializer.Serialize(extensionsInstalled);
        }
    }
}
