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

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ShelvesController : Controller
    {
        private readonly StorageDbContext _context;

        public ShelvesController(StorageDbContext context)
        {
            _context = context;
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

                return View(new DetailsModel { Shelf = shelf, Cases = cases });
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