using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OAHub.Base.Models.StorageModels;

namespace OAHub.Storage.Services
{
    public interface IStorageService
    {
        void AddFile(string shelfId, string caseId, IFormFile file);

        // Return the download link
        string DownloadFile(string shelfId, string caseId, string fileId);

        // Return the download link
        string DownloadCase(string shelfId, string caseId);

        void DeleteFile(string shelfId, string caseId, string fileId);
    }
}