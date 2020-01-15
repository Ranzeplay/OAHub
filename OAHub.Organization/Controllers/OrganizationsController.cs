using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Organization.Data;
using OAHub.Organization.Models;
using OAHub.Organization.Models.ViewModels.Orgainzations;

namespace OAHub.Organization.Controllers
{
    [Route("{Id}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationsController : Controller
    {
        private readonly OrganizationDbContext _context;

        public OrganizationsController(OrganizationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("/Create")]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var orgainzation = new Base.Models.OrganizationModels.Organization
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = model.Name,
                    Description = model.Description,
                    OfficialWebsite = model.OfficialWebsite,
                    CreateTime = DateTime.UtcNow,
                    Members = string.Empty,
                    FounderId = HttpContext.User.FindFirst("UserId").Value
                };

                _context.Add(orgainzation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Dashboard), new { id = orgainzation.Id });
            }

            return View(model);
        }

        [HttpGet]
        [Route("/Select")]
        public IActionResult Select()
        {
            var user = GetUserProfile();

            var organizations = _context.Organizations.Where(o => o.GetMembers().Contains(user.Id) || o.FounderId == user.Id).ToList();
            if(organizations == null)
            {
                organizations = new List<Base.Models.OrganizationModels.Organization>();
            }

            return View(organizations);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Dashboard(string id)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if(org != null)
            {
                return View(org);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Members(string id)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                var model = new List<OrganizationUser>();
                model.Add(_context.Users.FirstOrDefault(u => u.Id == org.FounderId));
                org.GetMembers().ForEach(userId =>
                {
                    model.Add(_context.Users.FirstOrDefault(u => u.Id == userId));
                });

                return View(model);
            }

            return NotFound();
        }

        private OrganizationUser GetUserProfile()
        {
            var user = new OrganizationUser();

            try
            {
                var id = HttpContext.User.FindFirst("UserId").Value;
                user = _context.Users.FirstOrDefault(u => u.Id == id);
            }
            catch
            {
                return null;
            }

            return user;
        }
    }
}