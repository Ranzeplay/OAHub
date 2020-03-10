using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using OAHub.Storage.Models.ViewModels.Shelves;
using OAHub.Storage.Services;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ShelvesController : Controller
    {
        private readonly StorageDbContext _context;
        private readonly IValidationService _validationService;

        public ShelvesController(StorageDbContext context)
        {
            _context = context;
            _validationService = new ValidationService(context);
        }

        [HttpGet]
        public IActionResult List()
        {
            var user = GetUserProfile();
            if (user != null)
            {
                var shelf = _context.Shelves.FirstOrDefault(s => s.Id == user.OwnedShelf);
                var model = new ListModel { Shelves = new List<Shelf> { shelf } };

                return View(model);
            }

            return Unauthorized();
        }

        public IActionResult Details(string shelfId)
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

                return View(new DetailsModel { Shelf = shelf, Cases = cases });
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Edit(string shelfId)
        {
            var user = GetUserProfile();
            if (_validationService.IsShelfExist(shelfId, out Shelf shelf, user))
            {
                var model = new EditModel
                {
                    Name = shelf.Name,
                    Description = shelf.Description
                };

                return View(model);
            }

            return Unauthorized();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string shelfId, EditModel model)
        {
            var user = GetUserProfile();
            if (_validationService.IsShelfExist(shelfId, out Shelf shelf, user))
            {
                shelf.Name = model.Name;
                shelf.Description = model.Description;

                _context.Shelves.Update(shelf);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { shelfId });
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