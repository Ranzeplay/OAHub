using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OAHub.Base.Models.StorageModels
{
    public class Case
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreateTime { get; set; }

        public string OwnedItems { get; set; }

        public List<string> GetOwnedItems()
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(OwnedItems);
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SetOwnedItems(List<string> vs)
        {
            OwnedItems = JsonSerializer.Serialize(vs);
        }
    }
}
