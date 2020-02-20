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
                    var projects = new List<ProjectViewModel>();
                    organization.GetProjectsId().ForEach(element =>
                    {
                        var currentProject = _context.Projects.FirstOrDefault(p => p.Id == element);
                        projects.Add(new ProjectViewModel
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

        [HttpGet]
        public async Task<IActionResult> Create(string orgId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    var members = await _organizationService.GetMembersAsync(orgId, _extensionProps.ExtId, organization.Secret, _extensionProps);
                    var model = new CreateModel
                    {
                        Name = string.Empty,
                        Description = string.Empty,
                        MembersAvailable = new List<Models.ViewModels.SelectMemberModel>()
                    };
                    members.ForEach(element =>
                    {
                        model.MembersAvailable.Add(new Models.ViewModels.SelectMemberModel { UserId = element.MemberId, DisplayName = element.FullName });
                    });

                    return View(model);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string orgId, CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetUserProfile();
                var organization = GetOrganization(orgId);
                if (organization != null)
                {
                    if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                    {
                        var project = new Project
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Name = model.Name,
                            Description = model.Description,
                            ManagerId = model.ManagerId,
                            Status = model.Status,
                            CreateTime = DateTime.UtcNow,
                            JobsId = string.Empty
                        };

                        // Register the project to the organization
                        var projectsCreated = organization.GetProjectsId();
                        projectsCreated.Add(project.Id);
                        organization.SetProjectsId(projectsCreated);
                        _context.WorkflowOrganizations.Update(organization);

                        _context.Projects.Add(project);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Details), new { orgId, projectId = project.Id });
                    }
                }

                return Unauthorized();
            }

            return RedirectToAction(nameof(Create), new { orgId });
        }

        public async Task<IActionResult> Details(string orgId, string projectId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
                    if (project != null)
                    {
                        //Get the manager of the project
                        var manager = _context.Users.FirstOrDefault(u => u.Id == project.ManagerId);

                        // Show jobs by JobViewModel
                        var projectJobs = new List<JobOverviewModel>();
                        project.GetJobsId().ForEach(element =>
                        {
                            var currentJob = _context.Jobs.FirstOrDefault(j => j.Id == element);
                            if (currentJob != null)
                            {
                                projectJobs.Add(new JobOverviewModel
                                {
                                    Id = currentJob.Id,
                                    Name = currentJob.Name,
                                    Description = currentJob.Description,
                                    Manager = _context.Users.FirstOrDefault(m => m.Id == currentJob.ManagerId),
                                    StepsCount = currentJob.GetSteps().Count,
                                    Status = currentJob.Status
                                });
                            }
                        });

                        return View(new DetailsModel
                        {
                            Project = new ProjectViewModel
                            {
                                Id = project.Id,
                                Name = project.Name,
                                Description = project.Description,
                                Manager = manager,
                                CreateTime = project.CreateTime,
                                Status = project.Status
                            },
                            Jobs = projectJobs
                        });
                    }
                }
            }

            return Unauthorized();
        }

        public IActionResult Delete(string orgId, string projectId)
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