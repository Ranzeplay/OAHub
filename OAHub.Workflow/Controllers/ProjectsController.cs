using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.WorkflowModels;
using OAHub.Workflow.Data;
using OAHub.Workflow.Models;

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProjectsController : Controller
    {
        private readonly WorkflowDbContext _context;
        private readonly ExtensionProps _extensionProps;

        public ProjectsController(WorkflowDbContext context, IOptions<ExtensionProps> extensionProps)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
        }

        public IActionResult List(string orgId)
        {
            var organization = GetOrganization(orgId);
            if(organization != null)
            {

            }

            return View();
        }

        public IActionResult Create(string orgId)
        {
            return View();
        }

        public IActionResult Details(string orgId)
        {
            return View();
        }

        public IActionResult Delete(string orgId, string prjId)
        {
            return View();
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

        private WorkflowOrganization GetOrganization(string orgId)
        {
            return _context.WorkflowOrganizations.FirstOrDefault(o => o.Id == orgId);
        }
    }
}