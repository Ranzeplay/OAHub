using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.StorageModels
{
    public class Shelf
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Owner { get; set; }

        public string CreateTime { get; set; }

        public string OwnedCases { get; set; }

        public List<string> GetOwnedCases()
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(OwnedCases);
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SetOwnedCases(List<string> vs)
        {
            OwnedCases = JsonSerializer.Serialize(vs);
        }
    }
}
