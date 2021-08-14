using OAHub.Base.Models.StorageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Models.ViewModels.Token
{
    public class ListModel
    {
        public List<StorageApiToken> OwnedTokens { get; set; }
    }
}
