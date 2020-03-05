using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Dashboard
{
    public class OverviewModel
    {
        // <shelfId, shelfName>
        public Dictionary<string, string> Shelves { get; set; }

        public double TotalFileSizeMB { get; set; }
    }
}
