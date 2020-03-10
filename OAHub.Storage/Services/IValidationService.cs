using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Services
{
    public interface IValidationService
    {
        bool IsShelfExist(string shelfId, out Shelf shelf, StorageUser user = null);

        bool IsCaseExist(string caseId, out Case @case, StorageUser user = null);

        bool IsCaseExist(string shelfId, string caseId, out Case @case, out Shelf shelf, StorageUser user = null);

        bool IsItemExist(string itemId, out Item item, StorageUser user = null);

        bool IsItemExist(string caseId, string itemId, out Item item, out Case @case, StorageUser user = null);

        bool IsItemExist(string shelfId, string caseId, string itemId, out Item item, out Case @case, out Shelf shelf, StorageUser user = null);
    }
}
