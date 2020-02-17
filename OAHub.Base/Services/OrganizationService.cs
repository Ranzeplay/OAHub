using OAHub.Base.Interfaces;
using OAHub.Base.Models.Extensions;
using OAHub.Base.Models.OrganizationModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAHub.Base.Services
{
    public class OrganizationService : IOrganizationService
    {
        public async Task<List<ApiMemberModel>> GetMembersAsync(string orgId, string extId, string orgSecret, string requestUrl)
        {
            var request = new HttpClient();
            var response = await request.GetAsync($"{requestUrl}".Replace("{OrgId}", orgId).Replace("{ExtId}", extId).Replace("{OrgSecret}", orgSecret));
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var decodedData = JsonSerializer.Deserialize<List<ApiMemberModel>>(content);
                return decodedData;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetOrganizationNameAsync(string orgId, ExtensionProps extensionProps)
        {
            var request = new HttpClient();
            var response = await request.GetAsync($"{extensionProps.ExtRootServerAddress.TrimEnd('/')}{extensionProps.GetOrganizationNamePath}".Replace("{orgId}", orgId));

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> HasViewPermission(string userId, string orgId, string extId, string orgSecret, ExtensionProps extensionProps)
        {
            var members = await GetMembersAsync(orgId, extId, orgSecret, $"{extensionProps.ExtRootServerAddress.TrimEnd('/')}/Api/GetMembers?orgId={{OrgId}}&extId={{ExtId}}&extSecret={{OrgSecret}}");
            return members.Exists(m => m.MemberId == userId);
        }
    }
}
