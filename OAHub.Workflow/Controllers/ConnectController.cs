using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Interfaces;
using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.WorkflowModels;
using OAHub.Workflow.Data;
using OAHub.Workflow.Models;
using OAHub.Workflow.Models.ViewModels.Connect;

namespace OAHub.Workflow.Controllers
{
    public class ConnectController : Controller
    {
        private readonly WorkflowDbContext _context;

        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;

        public ConnectController(IOptions<ExtensionProps> extensionProps, WorkflowDbContext context, IOrganizationService organizationService)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
            _organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string orgId, string extSecret)
        {
            var orgName = await _organizationService.GetOrganizationNameAsync(orgId, _extensionProps);

            if (orgName == "null")
            {
                return BadRequest();
            }

            return View(new SetupModel
            {
                OrganizationId = orgId,
                OrganizationSecret = extSecret,
                OrganizationName = orgName,
                ConfirmAction = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SetupModel model)
        {
            if (model.ConfirmAction)
            {
                _context.WorkflowOrganizations.Add(new WorkflowOrganization
                {
                    Id = model.OrganizationId,
                    Secret = model.OrganizationSecret
                });
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Go), new { orgId = model.OrganizationId });
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Go(string orgId)
        {
            var targetOrg = _context.WorkflowOrganizations.FirstOrDefault(o => o.Id == orgId);
            if (targetOrg != null)
            {
                var user = GetUserProfile();
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, targetOrg.Secret, _extensionProps))
                {
                    return RedirectToAction("Dashboard", "OrganizationRoot", new { orgId = targetOrg.Id });
                }

                return Unauthorized("Your are not in the organization");
            }

            return BadRequest("Organization not found");
        }

        private WorkflowUser GetUserProfile()
        {
            var user = new WorkflowUser();

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