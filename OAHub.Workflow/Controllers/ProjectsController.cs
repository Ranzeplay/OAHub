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
using OAHub.Workflow.Services;

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProjectsController : Controller
    {
        private readonly WorkflowDbContext _context;
        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;
        private readonly IValidationService _validationService;

        public ProjectsController(WorkflowDbContext context, IOptions<ExtensionProps> extensionProps, IOrganizationService organizationService, IValidationService validationService)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
            _organizationService = organizationService;
            _validationService = validationService;
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

        [HttpGet]
        public async Task<IActionResult> Edit(string orgId, string projectId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    
                    if (_validationService.IsProjectExists(projectId, out Project project))
                    {
                        var members = await _organizationService.GetMembersAsync(orgId, _extensionProps.ExtId, organization.Secret, _extensionProps);
                        var model = new EditModel
                        {
                            Name = project.Name,
                            Description = project.Description,
                            ManagerId = project.ManagerId,
                            Status = project.Status,
                            MembersAvailable = new List<Models.ViewModels.SelectMemberModel>()
                        };
                        members.ForEach(element =>
                        {
                            model.MembersAvailable.Add(new Models.ViewModels.SelectMemberModel { UserId = element.MemberId, DisplayName = element.FullName });
                        });

                        return View(model);
                    }
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string orgId, string projectId, EditModel model)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (_validationService.IsProjectExists(projectId, out Project project))
                    {
                        // Update project
                        project.Name = model.Name;
                        project.Description = model.Description;
                        project.ManagerId = model.ManagerId;
                        project.Status = model.Status;

                        _context.Projects.Update(project);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Details), new { orgId, projectId });
                    }
                }
            }

            return BadRequest();
        }


        public async Task<IActionResult> Delete(string orgId, string projectId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (_validationService.IsProjectExists(projectId, out Project project))
                    {
                        // Remove project
                        _context.Projects.Remove(project);

                        // Remove ProjectId from the organization
                        var projects = organization.GetProjectsId();
                        projects.Remove(projectId);
                        organization.SetProjectsId(projects);
                        _context.WorkflowOrganizations.Update(organization);

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Overview), new { orgId });
                    }
                }
            }

            return NotFound();
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