using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;

namespace OAHub.Storage.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageDbContext _context;

        public StorageService(StorageDbContext context)
        {
            _context = context;
        }

        public void AddFile(string shelfId, string caseId, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string shelfId, string caseId, string fileId)
        {
            throw new NotImplementedException();
        }

        public string DownloadCase(string shelfId, string caseId)
        {
            throw new NotImplementedException();
        }

        public string DownloadFile(string shelfId, string caseId, string fileId)
        {
            throw new NotImplementedException();
        }
    }
}