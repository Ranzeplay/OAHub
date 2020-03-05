using OAHub.Base.Models.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Files
{
    public class ListModel
    {
        public Case Case { get; set; }

        public List<ItemViewModel> Items { get; set; }
    }
}
