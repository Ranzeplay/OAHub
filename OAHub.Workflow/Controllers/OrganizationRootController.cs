using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Models.Extensions;
using OAHub.Workflow.Data;

namespace OAHub.Workflow.Controllers
{
    [Route("{OrgId}/Root")]
    public class OrganizationRootController : Controller
    {
        private readonly WorkflowDbContext _context;

        private readonly ExtensionProps _extensionProps;

        public OrganizationRootController(IOptions<ExtensionProps> extensionProps, WorkflowDbContext context)
        {
            _extensionProps = extensionProps.Value;
            _context = context;
        }

        public IActionResult Dashboard(string OrgId)
        {
            return View();
        }
    }
}