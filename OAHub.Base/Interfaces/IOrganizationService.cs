using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.OrganizationModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OAHub.Base.Interfaces
{
    public interface IOrganizationService
    {
        Task<List<ApiMemberModel>> GetMembersAsync(string orgId, string extId, string orgSecret, string requestUrl);
    }
}
