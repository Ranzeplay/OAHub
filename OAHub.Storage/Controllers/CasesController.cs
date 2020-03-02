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
            if(shelf != null && user.OwnedShelf == shelfId)
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