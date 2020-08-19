using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models
{
    public class AppSettings
    {
        public string StoragePath { get; set; }

        public string GetStoragePath() => Path.GetFullPath(StoragePath);
    }
}
