using OAHub.Base.Models;
using OAHub.Base.Models.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models
{
    public class ItemViewModel : Item
    {
        public FileSize Size { get; set; }
    }
}
