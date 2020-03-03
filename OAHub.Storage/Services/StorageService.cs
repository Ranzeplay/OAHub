using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;

namespace OAHub.Storage.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageDbContext _context;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        private readonly string _tempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

        public StorageService(StorageDbContext context)
        {
            _context = context;
        }

        public async Task AddFileAsync(string shelfId, string caseId, IFormFile file)
        {
            ValidateAndCreateDirectory(shelfId, caseId, out string path);
            var databaseItem = new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = file.FileName,
                CreateTime = DateTime.UtcNow
            };

            // Create single file directory
            var fileSaveDirectory = Path.Combine(path, databaseItem.Id);
            Directory.CreateDirectory(fileSaveDirectory);

            // Copy to destination
            var destinationFile = Path.Combine(fileSaveDirectory, file.FileName);
            var stream = new FileStream(destinationFile, FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            // Add item to the Case index
            var @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
            var items = @case.GetOwnedItems();
            items.Add(databaseItem.Id);
            @case.SetOwnedItems(items);
            _context.Cases.Update(@case);
            await _context.SaveChangesAsync();

            _context.Items.Add(databaseItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFileAsync(string shelfId, string caseId, string itemId)
        {
            ValidateAndCreateDirectory(shelfId, caseId, out string path);
            var databaseItem = _context.Items.FirstOrDefault(i => i.Id == itemId);
            var targetFolder = Path.Combine(path, itemId);
            if (File.Exists(Path.Combine(targetFolder, databaseItem.Name)) && databaseItem != null)
            {
                Directory.Delete(targetFolder, true);
                _context.Items.Remove(databaseItem);
                await _context.SaveChangesAsync();
            }
        }

        public FileStream DownloadFile(string shelfId, string caseId, string itemId)
        {
            ValidateAndCreateDirectory(shelfId, caseId, out string path);
            var databaseItem = _context.Items.FirstOrDefault(i => i.Id == itemId);
            var targetFile = Path.Combine(path, itemId, databaseItem.Name);
            if (File.Exists(targetFile) && databaseItem != null)
            {
                return File.OpenRead(targetFile);
            }

            return null;
        }

        public string ValidateAndCreateDirectory(string shelfId, string caseId, out string casePath)
        {
            casePath = Path.Combine(_storagePath, shelfId);
            if (!Directory.Exists(casePath))
            {
                Directory.CreateDirectory(casePath);
            }

            casePath = Path.Combine(casePath, caseId);
            if (!Directory.Exists(casePath))
            {
                Directory.CreateDirectory(casePath);
            }

            return casePath;
        }
    }
}