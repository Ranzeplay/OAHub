using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Status.Data;
using OAHub.Status.Models;
using OAHub.Status.Models.ViewModels.Manage;

namespace OAHub.Status.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ManageController : Controller
    {
        private readonly StatusDbContext _context;

        public ManageController(StatusDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = GetUserProfile();

            var tracks = _context.Tracks.Where(t => t.CreatedBy == user).ToList();

            return View(new IndexModel
            {
                Tracks = tracks != null ? tracks.ConvertAll(t => new TrackIndexModel(t)) : new List<TrackIndexModel>()
            });
        }

        public IActionResult AddStatus(string trackId)
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
