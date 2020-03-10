using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Services
{
    public class ValidationService : IValidationService
    {
        private readonly StorageDbContext _context;

        public ValidationService(StorageDbContext context)
        {
            _context = context;
        }

        public bool IsCaseExist(string caseId, out Case @case, StorageUser user = null)
        {
            @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
            if (@case != null)
            {
                if (user != null)
                {
                    var shelf = _context.Shelves.FirstOrDefault(s => s.GetOwnedCases().Contains(caseId));
                    return (shelf != null) && (user.OwnedShelf == shelf.Id);
                }

                return true;
            }

            return false;
        }

        public bool IsCaseExist(string shelfId, string caseId, out Case @case, out Shelf shelf, StorageUser user = null)
        {
            shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (shelf != null)
            {
                @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (shelf.GetOwnedCases().Contains(caseId) && @case != null)
                {
                    if (user != null)
                    {
                        return user.OwnedShelf == shelfId;
                    }

                    return true;
                }
            }

            @case = null;
            return false;
        }

        public bool IsItemExist(string itemId, out Item item, StorageUser user = null)
        {
            item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                if (user != null)
                {
                    var @case = _context.Cases.FirstOrDefault(c => c.GetOwnedItems().Contains(itemId));
                    if (@case != null)
                    {
                        var shelf = _context.Shelves.FirstOrDefault(s => s.GetOwnedCases().Contains(@case.Id));
                        if (shelf != null)
                        {
                            return user.OwnedShelf == shelf.Id;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsItemExist(string caseId, string itemId, out Item item, out Case @case, StorageUser user = null)
        {
            @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
            item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if(@case != null && item != null)
            {
                if (@case.GetOwnedItems().Contains(itemId))
                {
                    if(user != null)
                    {
                        var shelf = _context.Shelves.FirstOrDefault(s => s.GetOwnedCases().Contains(caseId));
                        if(shelf != null)
                        {
                            return user.OwnedShelf == shelf.Id;
                        }

                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool IsItemExist(string shelfId, string caseId, string itemId, out Item item, out Case @case, out Shelf shelf, StorageUser user = null)
        {
            shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
            item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if(shelf != null && @case != null && item != null)
            {
                if(shelf.GetOwnedCases().Contains(caseId) && @case.GetOwnedItems().Contains(itemId))
                {
                    if(user != null)
                    {
                        return user.OwnedShelf == shelfId;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool IsShelfExist(string shelfId, out Shelf shelf, StorageUser user = null)
        {
            shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (shelf != null)
            {
                if (user != null)
                {
                    return user.OwnedShelf == shelf.Id;
                }

                return true;
            }

            return false;
        }
    }
}
