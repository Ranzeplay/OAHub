using OAHub.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAHub.Storage.Models
{
    public class StorageUser : OAuthUser
    {
        public string OwnedShelf { get; set; }

        public string OwnedApiTokens { get; set; }

        public List<string> GetOwnedApiTokens()
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(OwnedApiTokens);
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SetOwnedApiTokens(List<string> vs)
        {
            OwnedApiTokens = JsonSerializer.Serialize(vs);
        }
    }
}
