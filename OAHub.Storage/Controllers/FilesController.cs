using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using OAHub.Storage.Models.ViewModels.Files;
using OAHub.Storage.Services;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class FilesController : Controller
    {
        private readonly StorageDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IValidationService _validationService;

        public FilesController(StorageDbContext context)
        {
            _context = context;
            _storageService = new StorageService(context);
            _validationService = new ValidationService(context);
        }

        public IActionResult List(string shelfId, string caseId)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out Shelf shelf, user))
            {
                var items = new List<ItemViewModel>();
                @case.GetOwnedItems().ForEach(element =>
                {
                    var item = _context.Items.FirstOrDefault(i => i.Id == element);
                    if (item != null)
                    {
                        var cItem = new ItemViewModel
                        {
                            Id = item.Id,
                            Name = item.Name,
                            CreateTime = item.CreateTime,
                            Size = new FileSize()
                        };

                        cItem.Size.Byte = _storageService.CalculateTotalSize(Path.Combine(Directory.GetCurrentDirectory(), "Storage", shelfId, caseId, item.Id), false);

                        items.Add(cItem);
                    }
                });

                return View(new ListModel { Case = @case, Items = items });
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string shelfId, string caseId, IFormFile file)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out Shelf shelf, user))
            {
                await _storageService.AddFileAsync(shelfId, caseId, file);

                return RedirectToAction(nameof(List), new { shelfId, caseId });
            }

            return Unauthorized();
        }

        public IActionResult Download(string shelfId, string caseId, string itemId)
        {
            var user = GetUserProfile();
            if (_validationService.IsItemExist(shelfId, caseId, itemId, out Item item, out Case @case, out Shelf shelf, user))
            {

                var stream = _storageService.DownloadFile(shelfId, caseId, itemId);
                return File(stream, "octlet/stream", item.Name);
            }

            return Unauthorized();
        }

        public async Task<IActionResult> Delete(string shelfId, string caseId, string itemId)
        {
            var user = GetUserProfile();
            if (_validationService.IsItemExist(shelfId, caseId, itemId, out Item item, out Case @case, out Shelf shelf, user))
            {
                await _storageService.DeleteFileAsync(shelfId, caseId, itemId);

                return RedirectToAction(nameof(List), new { shelfId, caseId });
            }

            return Unauthorized();
        }

        private StorageUser GetUserProfile()
        {
            try
            {
                var id = HttpContext.User.FindFirst("UserId").Value;
                return _context.Users.FirstOrDefault(u => u.Id == id);
            }
            catch
            {
                return null;
            }
        }
    }
}