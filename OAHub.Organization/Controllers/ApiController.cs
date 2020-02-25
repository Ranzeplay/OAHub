using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.Extensions;
using OAHub.Organization.Data;

namespace OAHub.Organization.Controllers
{
    public class ApiController : Controller
    {
        private readonly OrganizationDbContext _context;

        public ApiController(OrganizationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string GetOrganizationName(string orgId)
        {
            var targetOrganization = _context.Organizations.FirstOrDefault(o => o.Id == orgId);

            return (targetOrganization != null) ? targetOrganization.Name : "null";
        }

        public string GetMembers(string orgId, string extId, string extSecret)
        {
            var targetOrganization = _context.Organizations.FirstOrDefault(o => o.Id == orgId);
            if (targetOrganization != null)
            {
                var targetExtension = targetOrganization.GetExtensionsInstalled().FirstOrDefault(e => e.ExtId == extId);
                if (targetExtension != null && targetExtension.ExtSecret == extSecret)
                {
                    var model = new List<ApiMemberModel>();
                    targetOrganization.GetMembers().ForEach(element =>
                    {
                        var currentUser = _context.Users.FirstOrDefault(u => u.Id == element.UserId.ToString());
                        model.Add(new ApiMemberModel
                        {
                            MemberId = currentUser.Id,
                            FullName = currentUser.FullName,
                            Position = element.Position,
                            Email = currentUser.Email
                        });
                    });

                    return JsonSerializer.Serialize(model);
                }
            }

            return "null";
        }
    }
}