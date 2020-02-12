using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAHub.Base.Models;
using OAHub.Base.Utils;
using OAHub.Workflow.Data;
using OAHub.Workflow.Models;

namespace OAHub.Workflow.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthenticationInfomation _authenticationInfomation;
        private readonly WorkflowDbContext _context;

        public AuthController(IOptions<AuthenticationInfomation> authenticationInfomation, WorkflowDbContext context)
        {
            _authenticationInfomation = authenticationInfomation.Value;
            _context = context;
        }

        [HttpGet]
        public IActionResult PassportAuthorize()
        {
            string signInUrl = $"{_authenticationInfomation.PassportServerAddress}{_authenticationInfomation.PassportServerAuthorizePath}?" +
                $"AppId={_authenticationInfomation.AppId}&" +
                $"State={Guid.NewGuid().ToString("N")}&" +
                $"RedirectUri={_authenticationInfomation.AppServerAddress}{_authenticationInfomation.AppServerCallbackPath}";

            return Redirect(signInUrl);
        }

        public async Task<IActionResult> FinishAuth(string code)
        {
            var request = new HttpClient();
            var token = await request.GetAsync($"{_authenticationInfomation.PassportServerAddress}{_authenticationInfomation.PassportServerGetTokenPath}?" +
                $"appid={_authenticationInfomation.AppId}&appsecret={_authenticationInfomation.AppSecret}&code={code}");

            string tokenContent = await token.Content.ReadAsStringAsync();

            var profile = await request.GetAsync($"{_authenticationInfomation.PassportServerAddress}{_authenticationInfomation.PassportServerRequestProfilePath}?" +
                $"token={tokenContent}&appId={_authenticationInfomation.AppId}");

            var content = await profile.Content.ReadAsStringAsync();

            var oauthUser = JsonSerializer.Deserialize<OAuthUser>(Base64Tool.Decode(content));

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", oauthUser.Id),
                new Claim("Email", oauthUser.Email),
                new Claim("UserName", oauthUser.UserName),
                new Claim("PhoneNumber", oauthUser.PhoneNumber)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProps);
            var user = _context.Users.FirstOrDefault(u => u.Id == oauthUser.Id);
            if(user == null)
            {
                _context.Users.Add(new WorkflowUser { 
                    Id = oauthUser.Id,
                    UserName = oauthUser.UserName,
                    Email = oauthUser.Email,
                    PhoneNumber = oauthUser.PhoneNumber
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}