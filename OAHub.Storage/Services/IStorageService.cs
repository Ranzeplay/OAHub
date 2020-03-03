using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OAHub.Storage.Services
{
    public interface IStorageService
    {
        Task AddFileAsync(string shelfId, string caseId, IFormFile file);

        // Return the download link
        FileStream DownloadFile(string shelfId, string caseId, string itemId);

        Task DeleteFileAsync(string shelfId, string caseId, string itemId);

        void DeleteCase(string shelfId, string caseId);

        string ValidateOrCreateDirectory(string shelfId, string caseId, out string casePath);
    }
}