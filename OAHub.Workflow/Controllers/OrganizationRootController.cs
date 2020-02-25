using System;
using System.Collections.Generic;
using System.Linq;
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

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationRootController : Controller
    {
        private readonly WorkflowDbContext _context;

        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;

        public OrganizationRootController(IOptions<ExtensionProps> extensionProps, WorkflowDbContext context, IOrganizationService organizationService)
        {
            _extensionProps = extensionProps.Value;
            _context = context;
            _organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string orgId)
        {
            var user = GetUserProfile();
            var targetOrganization = GetOrganization(orgId);
            if(targetOrganization != null)
            {
                if(await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, targetOrganization.Secret, _extensionProps))
                {
                    return View();
                }

                return Unauthorized("You don't have permission to enter the organization");
            }

            return NotFound("Organization not found");
        }

        private WorkflowUser GetUserProfile()
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

        private WorkflowOrganization GetOrganization(string orgId)
        {
            return _context.WorkflowOrganizations.FirstOrDefault(o => o.Id == orgId);
        }
    }
}