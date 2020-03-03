using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAHub.Base.Models.StorageModels;

namespace OAHub.Storage.Models.ViewModels.Cases
{
    public class DetailsModel
    {
        public Case Case { get; set; }

        public List<Item> OwnedItems { get; set; }
    }
}
