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
using OAHub.Storage.Models.ViewModels.Account;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        private readonly StorageDbContext _context;

        public AccountController(StorageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Initial()
        {
            var user = GetUserProfile();
            if (user != null)
            {
                if (user.OwnedShelf == null)
                {
                    return View(new InitialModel
                    {
                        ShelfName = $"{user.UserName}\'s Shelf"
                    });
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Initial(InitialModel model)
        {
            var user = GetUserProfile();
            if (user != null)
            {
                if (user.OwnedShelf == null)
                {
                    if (ModelState.IsValid)
                    {
                        var shelf = new Shelf
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Name = model.ShelfName,
                            Description = model.ShelfDescrption,
                            CreateTime = DateTime.UtcNow
                        };

                        _context.Shelves.Add(shelf);
                        user.OwnedShelf = shelf.Id;
                        _context.Users.Update(user);

                        await _context.SaveChangesAsync();

                        return RedirectToAction("Overview", "Dashboard", new { userId = user.Id });
                    }

                    return View(model);
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