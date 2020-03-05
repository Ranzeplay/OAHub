using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public FilesController(StorageDbContext context)
        {
            _context = context;
            _storageService = new StorageService(context);
        }

        public IActionResult List(string shelfId, string caseId)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (user.OwnedShelf == shelfId && shelf != null)
            {
                var @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (shelf.GetOwnedCases().Contains(caseId) && @case != null)
                {
                    var items = new List<ItemViewModel>();
                    @case.GetOwnedItems().ForEach(element =>
                    {
                        var item = _context.Items.FirstOrDefault(i => i.Id == element);
                        if (item != null)
                        {
                            items.Add(new ItemViewModel
                            {
                                Id = item.Id,
                                Name = item.Name,
                                CreateTime = item.CreateTime
                            });
                        }
                    });

                    return View(new ListModel { Case = @case, Items = items });
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string shelfId, string caseId, IFormFile file)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (user.OwnedShelf == shelfId && shelf != null)
            {
                var @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (shelf.GetOwnedCases().Contains(caseId) && @case != null)
                {
                    await _storageService.AddFileAsync(shelfId, caseId, file);

                    return RedirectToAction(nameof(List), new { shelfId, caseId });
                }
            }

            return Unauthorized();
        }

        public IActionResult Download(string shelfId, string caseId, string itemId)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (user.OwnedShelf == shelfId && shelf != null)
            {
                var @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (shelf.GetOwnedCases().Contains(caseId) && @case != null)
                {
                    var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
                    if (@case.GetOwnedItems().Contains(itemId) && item != null)
                    {
                        var stream = _storageService.DownloadFile(shelfId, caseId, itemId);
                        return File(stream, "octlet/stream", item.Name);
                    }
                }
            }

            return Unauthorized();
        }

        public async Task<IActionResult> Delete(string shelfId, string caseId, string itemId)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (user.OwnedShelf == shelfId && shelf != null)
            {
                var @case = _context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (shelf.GetOwnedCases().Contains(caseId) && @case != null)
                {
                    var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
                    if (@case.GetOwnedItems().Contains(itemId) && item != null)
                    {
                        await _storageService.DeleteFileAsync(shelfId, caseId, itemId);

                        return RedirectToAction(nameof(List), new { shelfId, caseId });
                    }
                }
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