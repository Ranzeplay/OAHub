using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.Extensions;
using OAHub.Organization.Data;
using OAHub.Organization.Models;
using OAHub.Organization.Models.ViewModels.Extensions;

namespace OAHub.Organization.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ExtensionsController : Controller
    {
        private readonly OrganizationDbContext _context;

        public ExtensionsController(OrganizationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var extensions = _context.Extensions.ToList();
            var model = new List<IndexViewModel>();
            foreach (var ext in extensions)
            {
                var author = _context.Users.FirstOrDefault(u => u.Id == ext.AuthorId);
                model.Add(new IndexViewModel
                {
                    CreateBy = author.UserName,
                    Name = ext.Name,
                    Id = ext.Id
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(NewModel model)
        {
            var user = GetUserProfile();

            if (ModelState.IsValid)
            {
                var ext = new Extension
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = model.Name,
                    Description = model.Description,
                    WebSite = model.WebSite,
                    OrganizationRootUri = model.OrganizationRootUri,
                    CreateDashboardUri = model.CreateDashboardUri,
                    AuthorId = user.Id
                };

                _context.Extensions.Add(ext);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Details(string extId)
        {
            var ext = _context.Extensions.FirstOrDefault(e => e.Id == extId);
            if (ext != null)
            {
                var author = _context.Users.FirstOrDefault(u => u.Id == ext.AuthorId);

                var model = new DetailsModel
                {
                    Extension = ext,
                    AuthorName = author.FullName,
                    UserJoinedOrganizations = new List<KeyValuePair<string, string>>()
                };

                var user = GetUserProfile();
                foreach (var org in _context.Organizations.ToList())
                {
                    if (((org.GetMembers().FirstOrDefault(m => m.UserId == user.Id) != null) || org.FounderId == user.Id) && !model.UserJoinedOrganizations.Exists(e => e.Key == org.Id))
                    {
                        model.UserJoinedOrganizations.Add(new KeyValuePair<string, string>(org.Id, org.Name));
                    }
                }

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddToOrg(OrgAccessModel model)
        {
            var user = GetUserProfile();
            var organization = _context.Organizations.FirstOrDefault(o => o.Id == model.OrganizationId);
            var extension = _context.Extensions.FirstOrDefault(e => e.Id == model.ExtensionId);
            if (organization != null && extension != null)
            {
                if (organization.GetMembers().Exists(m => m.UserId == user.Id))
                {
                    var extensionsInstalled = organization.GetExtensionsInstalled();
                    extensionsInstalled.Add(new ExtensionCredential
                    {
                        ExtId = extension.Id,
                        ExtSecret = Guid.NewGuid().ToString("N")
                    });
                    organization.SetExtensionsInstalled(extensionsInstalled);
                    _context.Organizations.Update(organization);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Dashboard", "Organizations", new { id = organization.Id });
                }
            }

            return BadRequest();
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