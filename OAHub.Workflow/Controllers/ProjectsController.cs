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
using OAHub.Workflow.Models.ViewModels.Projects;

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProjectsController : Controller
    {
        private readonly WorkflowDbContext _context;
        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;

        public ProjectsController(WorkflowDbContext context, IOptions<ExtensionProps> extensionProps, IOrganizationService organizationService)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
            _organizationService = organizationService;
        }

        public async Task<IActionResult> Overview(string orgId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    var projects = new List<Models.ProjectViewModel>();
                    organization.GetProjectsId().ForEach(element =>
                    {
                        var currentProject = _context.Projects.FirstOrDefault(p => p.Id == element);
                        projects.Add(new Models.ProjectViewModel
                        {
                            Id = currentProject.Id,
                            Name = currentProject.Name,
                            Description = currentProject.Description,
                            CreateTime = currentProject.CreateTime,
                            Manager = _context.Users.FirstOrDefault(u => u.Id == currentProject.ManagerId),
                            Status = currentProject.Status
                        });
                    });
                    return View(new OverviewModel
                    {
                        OrganizationName = await _organizationService.GetOrganizationNameAsync(orgId, _extensionProps),
                        Projects = projects
                    });
                }

                return Unauthorized();
            }

            return NotFound();
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