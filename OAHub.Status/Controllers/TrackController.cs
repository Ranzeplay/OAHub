using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Status.Data;
using OAHub.Status.Models;
using OAHub.Status.Models.ViewModels.Track;

namespace OAHub.Status.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TrackController : Controller
    {
        private readonly StatusDbContext _context;

        public TrackController(StatusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            var user = GetUserProfile();

            if (ModelState.IsValid)
            {
                var track = new Track
                {
                    Name = model.Name,
                    Posts = new List<Post>(),
                    CreatedBy = user,
                };

                _context.Tracks.Add(track);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }

            return View();
        }

        public IActionResult Delete(string trackId)
        {
            return View();
        }

        public IActionResult Summary(string trackId)
        {
            return View();
        }

        private StatusUser GetUserProfile()
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
