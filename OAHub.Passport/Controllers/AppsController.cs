using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models;
using OAHub.Passport.Data;
using OAHub.Passport.Models.ViewModels.Apps;

namespace OAHub.Passport.Controllers
{
    [Authorize]
    public class AppsController : Controller
    {
        private readonly PassportDbContext _context;
        private readonly UserManager<OAUser> _userManager;

        public AppsController(PassportDbContext context, UserManager<OAUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(NewAppModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var app = new App
                {
                    AppId = Guid.NewGuid().ToString("N"),
                    AppSecret = Guid.NewGuid().ToString("N"),
                    CreateTime = DateTime.UtcNow,
                    Name = model.Name,
                    ManagerId = user.Id,
                    UsersAuthorized = string.Empty
                };

                _context.Apps.Add(app);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { app.AppId });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string AppId)
        {
            var app = await GetAppAsync(AppId);
            if(app != null)
            {
                var user = await _userManager.GetUserAsync(User);

                if (app.ManagerId == user.Id)
                {
                    return View(app);
                }

                return Unauthorized();
            }

            return NotFound();
        }

        private async Task<App> GetAppAsync(string appId)
        {
            return await _context.Apps.FirstOrDefaultAsync(a => a.AppId == appId);
        }
    }
}