using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using OAHub.Storage.Models.ViewModels.Dashboard;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DashboardController : Controller
    {
        private readonly StorageDbContext _context;

        public DashboardController(StorageDbContext context)
        {
            _context = context;
        }

        public IActionResult Overview(string userId)
        {
            var user = GetUserProfile();
            if (user.OwnedShelf == null)
            {
                return RedirectToAction("Initial", "Account", new { userId });
            }

            return View(new OverviewModel
            {
                TotalFileSizeMB = 5.22,
                Shelves = new Dictionary<string, string>()
            });
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