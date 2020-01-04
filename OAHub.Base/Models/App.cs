using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAHub.Base.Models
{
    public class App
    {
        [Key]
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public string ManagerId { get; set; }

        public string UsersAuthorized { get; set; }

        // Returns UserId
        public List<string> GetUsersAuthorized()
        {
            if (!string.IsNullOrEmpty(UsersAuthorized))
            {
                return JsonSerializer.Deserialize<List<string>>(UsersAuthorized);
            }

            return new List<string>();
        }

        public void SetUsersAuthorized(List<string> users)
        {
            UsersAuthorized = JsonSerializer.Serialize(users);
        }
    }
}
