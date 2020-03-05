using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Cases
{
    public class CreateModel
    {
        // <ShelfId, ShelfName>
        public KeyValuePair<string, string> RootShelf { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
