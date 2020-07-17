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

        [HttpGet]
        public IActionResult Delete(string trackId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string trackId, DeleteModel model)
        {
            var user = GetUserProfile();
            var track = _context.Tracks.Where(t => t.CreatedBy == user).FirstOrDefault(t => t.Id == Guid.Parse(trackId));
            if (track != null)
            {
                if (model.Confirm && model.TrackName.ToLower() == "yes")
                {
                    _context.Tracks.Remove(track);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(ManageController.Index), "Manage");
                }
            }

            return View();
        }

        public IActionResult Summary(string trackId)
        {
            var user = GetUserProfile();
            var track = _context.Tracks.FirstOrDefault(t => t.Id == Guid.Parse(trackId));
            if (track != null)
            {
                var posts = _context.Posts.Where(p => p.ForTrack == track);

                // "Where" command cannot find the elements of none, so it needs to prevent these errors\
                var headingPost = new Post();
                try
                {
                    headingPost = posts.Where(p => p.ShowOnHeader).ToList().OrderByDescending(t => t.PublishTime).First();
                }
                catch
                {
                    headingPost = Post.BlankPost();
                }

                return View(new SummaryModel
                {
                    Name = track.Name,
                    RecentPosts = posts.Count() > 0 ? posts.OrderByDescending(t => t.PublishTime).ToList() : new List<Post>(),
                    HeadingPost = headingPost
                });
            }

            return NotFound();
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
