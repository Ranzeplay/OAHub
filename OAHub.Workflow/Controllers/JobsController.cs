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
using OAHub.Workflow.Models.ViewModels.Jobs;

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class JobsController : Controller
    {
        private readonly WorkflowDbContext _context;
        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;

        public JobsController(WorkflowDbContext context, IOptions<ExtensionProps> extensionProps, IOrganizationService organizationService)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
            _organizationService = organizationService;
        }

        public async Task<IActionResult> List(string orgId, string projectId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    var project = GetProject(orgId, projectId);
                    var model = new ListModel
                    {
                        OrganizationName = await _organizationService.GetOrganizationNameAsync(orgId, _extensionProps),
                        ProjectName = project.Name,
                        Jobs = new List<JobOverviewModel>()
                    };
                    project.GetJobsId().ForEach(element =>
                    {
                        var job = GetJob(orgId, projectId, element);
                        if (job != null)
                        {
                            model.Jobs.Add(new JobOverviewModel
                            {
                                Id = job.Id,
                                Name = job.Name,
                                Description = job.Description,
                                Manager = _context.Users.FirstOrDefault(u => u.Id == job.ManagerId),
                                Status = job.Status,
                                StepsCount = job.GetSteps().Count
                            });
                        }
                    });

                    return View(model);
                }
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CreateJob(string orgId, string projectId)
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

                    ViewData["projectId"] = projectId;

                    return View(model);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(string orgId, string projectId, CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetUserProfile();
                var organization = GetOrganization(orgId);
                if (organization != null)
                {
                    if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                    {
                        var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
                        if (organization.GetProjectsId().Contains(projectId) && project != null)
                        {
                            var job = new Job
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = model.Name,
                                Description = model.Description,
                                ManagerId = model.ManagerId,
                                Status = model.Status,
                            };

                            var jobsId = project.GetJobsId();
                            jobsId.Add(job.Id);
                            project.SetJobsId(jobsId);

                            _context.Jobs.Add(job);
                            _context.Projects.Update(project);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Details), new { orgId, projectId, jobId = job.Id });
                        }
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string orgId, string projectId, string jobId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
                    if (organization.GetProjectsId().Contains(projectId) && project != null)
                    {
                        var job = _context.Jobs.FirstOrDefault(j => j.Id == jobId);
                        if (project.GetJobsId().Contains(jobId) && job != null)
                        {
                            var model = new JobStepsViewModel
                            {
                                Id = job.Id,
                                Name = job.Name,
                                Description = job.Description,
                                Manager = _context.Users.FirstOrDefault(u => u.Id == job.ManagerId),
                                Status = job.Status,
                                Steps = new List<StepViewModel>(),
                                StepsCount = job.GetSteps().Count
                            };

                            job.GetSteps().ForEach(element =>
                            {
                                model.Steps.Add(new StepViewModel
                                {
                                    Name = element.Name,
                                    Description = element.Description,
                                    AssigneesCount = element.GetAssigneesId().LongCount(),
                                    Status = element.Status
                                });
                            });

                            return View(model);
                        }
                    }
                }
            }

            return NotFound();
        }

        public IActionResult CreateStep(string orgId, string projectId, string jobId)
        {
            return View();
        }

        public IActionResult ViewStep(string orgId, string projectId, string jobId)
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

        private Project GetProject(string orgId, string projectId)
        {
            var targetOrg = GetOrganization(orgId);
            if (targetOrg != null)
            {
                if (targetOrg.GetProjectsId().Contains(projectId))
                {
                    return _context.Projects.FirstOrDefault(p => p.Id == projectId);
                }
            }
            return null;
        }

        private Job GetJob(string orgId, string projectId, string jobId)
        {
            var project = GetProject(orgId, jobId);
            if (project != null)
            {
                if (project.GetJobsId().Contains(jobId))
                {
                    return _context.Jobs.FirstOrDefault(j => j.Id == jobId);
                }
            }

            return null;
        }
    }
}