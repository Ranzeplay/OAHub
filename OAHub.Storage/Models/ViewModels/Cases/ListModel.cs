using OAHub.Base.Models.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Cases
{
    public class ListModel
    {
        public Shelf Shelf { get; set; }

        public List<Case> Cases { get; set; }
    }
}
