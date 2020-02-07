using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.OrganizationModels;
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

                orgainzation.SetMembers(new List<Member> {
                    new Member
                    {
                        UserId = orgainzation.FounderId,
                        JoinAt = DateTime.UtcNow,
                        Position = "Founder",
                        IsLocked = true
                    }
                });

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

            var organizations = new List<Base.Models.OrganizationModels.Organization>();
            foreach (var org in _context.Organizations.ToList())
            {
                if (org.GetMembers().Exists(m => m.UserId == user.Id))
                {
                    organizations.Add(org);
                }
            }

            return View(organizations);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Dashboard(string id)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
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
                var model = new List<MemberModel>();
                foreach (var member in org.GetMembers())
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == member.UserId);
                    model.Add(new MemberModel
                    {
                        User = user,
                        Member = member
                    });
                }

                return View(model);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("[action]/{memberId}")]
        public IActionResult Member(string id, string memberId)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == memberId);
                if (user != null && org.GetMembers().Exists(m => m.UserId == memberId))
                {
                    var model = new MemberModel
                    {
                        User = user,
                        Member = org.GetMembers().FirstOrDefault(m => m.UserId == user.Id)
                    };
                    return View(model);
                }
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[action]/{memberId}")]
        public async Task<IActionResult> Member(string id, string memberId, MemberModel model)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == memberId);
                var members = org.GetMembers();
                if (user != null && members.Exists(m => m.UserId == memberId))
                {
                    // Get member by Id and update member info
                    // Then update the list of member, replace old as new
                    var member = members.FirstOrDefault(m => m.UserId == model.Member.UserId);
                    member.Position = model.Member.Position;
                    members[members.IndexOf(members.FirstOrDefault(m => m.UserId == model.Member.UserId))] = member;
                    org.SetMembers(members);

                    // Update database
                    _context.Organizations.Update(org);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Member), new { id, memberId });
                }
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Invite(string id, InviteModel model)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                if (ModelState.IsValid)
                {
                    var targetUser = _context.Users.FirstOrDefault(t => t.Id == model.UserId);
                    if (targetUser != null)
                    {
                        var newMember = new Member
                        {
                            UserId = model.UserId,
                            Position = model.Position,
                            JoinAt = DateTime.UtcNow,
                            IsLocked = false
                        };

                        var members = org.GetMembers();
                        members.Add(newMember);
                        org.SetMembers(members);

                        _context.Organizations.Update(org);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Members), new { id });
            }

            return NotFound();
        }

        [Route("[action]")]
        public IActionResult InstalledExtensions(string id)
        {
            var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                var model = new List<Extension>();
                foreach (var ext in org.GetExtensionsInstalled())
                {
                    model.Add(_context.Extensions.FirstOrDefault(e => e.Id == ext.ExtId));
                }

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