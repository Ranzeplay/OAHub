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
using OAHub.Storage.Models.ViewModels.Cases;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CasesController : Controller
    {
        private readonly StorageDbContext _context;

        public CasesController(StorageDbContext context)
        {
            _context = context;
        }

        public IActionResult List(string shelfId)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (shelf != null && user.OwnedShelf == shelfId)
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
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (shelf != null && user.OwnedShelf == shelfId)
            {
                return View(new CreateModel { RootShelf = new KeyValuePair<string, string>(shelf.Id, shelf.Name) });
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string shelfId, CreateModel model)
        {
            var user = GetUserProfile();
            var shelf = _context.Shelves.FirstOrDefault(s => s.Id == shelfId);
            if (shelf != null && user.OwnedShelf == shelfId)
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