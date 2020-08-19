using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using OAHub.Storage.Models.ViewModels.Cases;
using OAHub.Storage.Services;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CasesController : Controller
    {
        private readonly StorageDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IValidationService _validationService;
        private readonly AppSettings _appSettings;

        public CasesController(StorageDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _storageService = new StorageService(context, appSettings);
            _validationService = new ValidationService(context);
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult List(string shelfId)
        {
            var user = GetUserProfile();
            if (_validationService.IsShelfExist(shelfId, out Shelf shelf, user))
            {
                var cases = new List<Case>();
                shelf.GetOwnedCases().ForEach(element =>
                {
                    var @case = _context.Cases.FirstOrDefault(c => c.Id == element);
                    if (@case != null)
                    {
                        cases.Add(@case);
                    }
                });

                return View(new ListModel
                {
                    Shelf = shelf,
                    Cases = cases
                });
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Create(string shelfId)
        {
            var user = GetUserProfile();
            if (_validationService.IsShelfExist(shelfId, out Shelf shelf, user))
            {
                return View(new CreateModel { RootShelf = new KeyValuePair<string, string>(shelf.Id, shelf.Name) });
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string shelfId, CreateModel model)
        {
            var user = GetUserProfile();
            if (_validationService.IsShelfExist(shelfId, out Shelf shelf, user))
            {
                if (ModelState.IsValid)
                {
                    var @case = new Case
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Name = model.Name,
                        Description = model.Description,
                        CreateTime = DateTime.UtcNow,
                    };

                    // Add case to database
                    _context.Cases.Add(@case);

                    // Make target shelf own the case
                    var cases = shelf.GetOwnedCases();
                    cases.Add(@case.Id);
                    shelf.SetOwnedCases(cases);
                    _context.Shelves.Update(shelf);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(List), new { shelfId });
                }

                return View(model);
            }

            return Unauthorized();
        }

        public IActionResult Details(string shelfId, string caseId)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out Shelf shelf, user))
            {
                var model = new DetailsModel
                {
                    Case = @case,
                    OwnedItems = new List<Item>()
                };

                @case.GetOwnedItems().ForEach(element =>
                {
                    var item = _context.Items.FirstOrDefault(i => i.Id == element);
                    if (item != null)
                    {
                        model.OwnedItems.Add(item);
                    }
                });

                return View(model);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Edit(string shelfId, string caseId)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out _, user))
            {
                var model = new EditModel
                {
                    Name = @case.Name,
                    Description = @case.Description
                };

                return View(model);
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string shelfId, string caseId, EditModel model)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out Shelf shelf, user))
            {
                @case.Name = model.Name;
                @case.Description = model.Description;

                _context.Cases.Update(@case);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { shelfId, caseId });
            }

            return Unauthorized();
        }

        public async Task<IActionResult> Delete(string shelfId, string caseId)
        {
            var user = GetUserProfile();
            if (_validationService.IsCaseExist(shelfId, caseId, out Case @case, out _, user))
            {
                _context.Cases.Remove(@case);
                _storageService.DeleteCase(shelfId, caseId);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Shelves", new { shelfId });
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