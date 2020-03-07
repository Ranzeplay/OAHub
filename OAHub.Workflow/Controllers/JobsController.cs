using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
using OAHub.Workflow.Models.ViewModels;
using OAHub.Workflow.Models.ViewModels.Jobs;
using OAHub.Workflow.Services;

namespace OAHub.Workflow.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class JobsController : Controller
    {
        private readonly WorkflowDbContext _context;
        private readonly ExtensionProps _extensionProps;
        private readonly IOrganizationService _organizationService;
        private readonly IValidationService _validationService;

        public JobsController(WorkflowDbContext context, IOptions<ExtensionProps> extensionProps, IOrganizationService organizationService, IValidationService validationService)
        {
            _context = context;
            _extensionProps = extensionProps.Value;
            _organizationService = organizationService;
            _validationService = validationService;
        }

        public async Task<IActionResult> List(string orgId, string projectId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (_validationService.IsProjectExists(projectId, out Project project))
                    {
                        var model = new ListModel
                        {
                            OrganizationName = await _organizationService.GetOrganizationNameAsync(orgId, _extensionProps),
                            ProjectName = project.Name,
                            Jobs = new List<JobOverviewModel>()
                        };
                        project.GetJobsId().ForEach(element =>
                        {

                            if (_validationService.IsJobExists(element, out Job job))
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
                    var model = new CreateJobModel
                    {
                        Name = string.Empty,
                        Description = string.Empty,
                        MembersAvailable = new List<SelectMemberModel>()
                    };
                    members.ForEach(element =>
                    {
                        model.MembersAvailable.Add(new SelectMemberModel { UserId = element.MemberId, DisplayName = element.FullName });
                    });

                    return View(model);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(string orgId, string projectId, CreateJobModel model)
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

                    return Unauthorized();
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
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
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
                                    NameEncoded = element.Id,
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

        [HttpGet]
        public async Task<IActionResult> EditJob(string orgId, string projectId, string jobId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var members = await _organizationService.GetMembersAsync(orgId, _extensionProps.ExtId, organization.Secret, _extensionProps);
                            var model = new EditJobModel
                            {
                                Name = job.Name,
                                Description = job.Description,
                                ManagerId = job.ManagerId,
                                Status = job.Status,
                                MembersAvailable = new List<SelectMemberModel>()
                            };
                            members.ForEach(element =>
                            {
                                model.MembersAvailable.Add(new SelectMemberModel { UserId = element.MemberId, DisplayName = element.FullName });
                            });

                            return View(model);
                        }
                    }
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> EditJob(string orgId, string projectId, string jobId, EditJobModel model)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            // Update
                            job.Name = model.Name;
                            job.Description = model.Description;
                            job.ManagerId = model.ManagerId;
                            job.Status = model.Status;

                            // Apply changes
                            _context.Jobs.Update(job);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Details), new { orgId, projectId, jobId });
                        }
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> DeleteJob(string orgId, string projectId, string jobId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            _context.Jobs.Remove(job);

                            // Remove the JobId from the project
                            var registeredJobs = project.GetJobsId();
                            registeredJobs.Remove(jobId);
                            project.SetJobsId(registeredJobs);
                            _context.Update(project);

                            // Remove all Jobs under the project
                            project.GetJobsId().ForEach(element =>
                            {
                                _context.Jobs.Remove(_context.Jobs.FirstOrDefault(j => j.Id == element));
                            });

                            // Apply changes
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Details", "Projects", new { orgId, projectId });
                        }
                    }
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep(string orgId, string projectId, string jobId)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var model = new CreateStepModel
                            {
                                Assignees = new List<AssignerViewModel>()
                            };

                            var members = await _organizationService.GetMembersAsync(orgId, _extensionProps.ExtId, organization.Secret, _extensionProps);
                            members.ForEach(element =>
                            {
                                model.Assignees.Add(new AssignerViewModel { AssignerId = element.MemberId, ShowName = element.FullName, IsSelected = false });
                            });

                            return View(model);
                        }
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStep(string orgId, string projectId, string jobId, CreateStepModel model)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var steps = job.GetSteps();

                            var stepIdentifier = Guid.NewGuid().ToString("N");
                            var newStep = new Step
                            {
                                // Just use first 25 chars to be the Id of the step
                                Id = stepIdentifier.Substring(0, (stepIdentifier.Length > 12) ? 12 : stepIdentifier.Length),
                                Name = model.Name,
                                Description = model.Description,
                                Status = model.Status
                            };
                            newStep.SetAssigneesId(model.Assignees.Select(q => q.AssignerId).ToList());
                            steps.Add(newStep);
                            job.SetSteps(steps);

                            _context.Jobs.Update(job);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Details), new { orgId, projectId, jobId });
                        }
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> ViewStep(string orgId, string projectId, string jobId, string stepNameEncoded)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var step = job.GetSteps().FirstOrDefault(s => s.Id == stepNameEncoded);
                            var model = new StepViewModel
                            {
                                NameEncoded = step.Id,
                                Name = step.Name,
                                Description = step.Description,
                                Status = step.Status,
                                AssigneesCount = step.GetAssigneesId().Count,
                                Assignees = new List<WorkflowUser>()
                            };

                            step.GetAssigneesId().ForEach(element =>
                            {
                                var member = _context.Users.FirstOrDefault(m => m.Id == element);
                                if (element != null)
                                {
                                    model.Assignees.Add(member);
                                }
                            });

                            return View(model);
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditStep(string orgId, string projectId, string jobId, string stepNameEncoded)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var step = job.GetSteps().FirstOrDefault(s => s.Id == stepNameEncoded);

                            var model = new EditStepModel
                            {
                                Name = step.Name,
                                Description = step.Description,
                                Status = step.Status,
                                Assignees = new List<AssignerViewModel>()
                            };

                            var members = await _organizationService.GetMembersAsync(orgId, _extensionProps.ExtId, organization.Secret, _extensionProps);
                            members.ForEach(element =>
                            {
                                model.Assignees.Add(new AssignerViewModel { AssignerId = element.MemberId, ShowName = element.FullName, IsSelected = step.GetAssigneesId().Contains(element.MemberId) });
                            });

                            return View(model);
                        }
                    }
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> EditStep(string orgId, string projectId, string jobId, string stepNameEncoded, EditStepModel model)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var steps = job.GetSteps();
                            var step = steps.FirstOrDefault(s => s.Id == stepNameEncoded);
                            var index = steps.IndexOf(step);

                            // Update the step
                            step.Name = model.Name;
                            step.Description = model.Description;
                            step.Status = model.Status;
                            step.SetAssigneesId(model.Assignees.Select(a => a.AssignerId).ToList());

                            //Apply changes
                            steps[index] = step;
                            job.SetSteps(steps);
                            _context.Jobs.Update(job);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Details), new { orgId, projectId, jobId });
                        }
                    }
                }
            }

            return Unauthorized();
        }

        public async Task<IActionResult> DeleteStep(string orgId, string projectId, string jobId, string stepNameEncoded)
        {
            var user = GetUserProfile();
            var organization = GetOrganization(orgId);
            if (organization != null)
            {
                if (await _organizationService.HasViewPermission(user.Id, orgId, _extensionProps.ExtId, organization.Secret, _extensionProps))
                {
                    if (organization.GetProjectsId().Contains(projectId) && _validationService.IsProjectExists(projectId, out Project project))
                    {
                        if (project.GetJobsId().Contains(jobId) && _validationService.IsJobExists(jobId, out Job job))
                        {
                            var steps = job.GetSteps();
                            var step = steps.FirstOrDefault(s => s.Id == stepNameEncoded);
                            steps.Remove(step);

                            job.SetSteps(steps);
                            _context.Jobs.Update(job);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Details), new { orgId, projectId, jobId });
                        }
                    }
                }
            }

            return BadRequest();
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