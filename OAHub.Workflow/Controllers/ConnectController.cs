using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.WorkflowModels;
using OAHub.Workflow.Data;
using OAHub.Workflow.Models.ViewModels.Connect;

namespace OAHub.Workflow.Controllers
{
    public class ConnectController : Controller
    {
        private readonly WorkflowDbContext _context;

        private readonly ExtensionProps _extensionProps;

        public ConnectController(IOptions<ExtensionProps> extensionProps, WorkflowDbContext context)
        {
            _extensionProps = extensionProps.Value;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string orgId, string extSecret)
        {
            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync($"{_extensionProps.ExtRootServerAddress.TrimEnd('/')}{_extensionProps.GetOrganizationNamePath}".Replace("{orgId}", orgId));

            var orgName = await httpResponse.Content.ReadAsStringAsync();
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
        public IActionResult Go(string orgId)
        {
            var targetOrg = _context.WorkflowOrganizations.FirstOrDefault(o => o.Id == orgId);
            if(targetOrg != null)
            {
                return RedirectToAction("Dashboard", "OrganizarionRoot", new { OrgId = orgId });
            }

            return BadRequest("Organization not found");
        }
    }
}