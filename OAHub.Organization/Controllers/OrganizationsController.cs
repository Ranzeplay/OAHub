using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Organization.Data;
using OAHub.Organization.Models.ViewModels.Orgainzations;

namespace OAHub.Organization.Controllers
{
    [Route("{Id}")]
    [Authorize(CookieAuthenticationDefaults.AuthenticationScheme)]
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
    }
}