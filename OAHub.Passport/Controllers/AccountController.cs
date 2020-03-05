using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models;
using OAHub.Base.Utils;
using OAHub.Passport.Data;
using OAHub.Passport.Models.ViewModels.Account;
using OAHub.Passport.Services;

namespace OAHub.Passport.Controllers
{
    public class AccountController : Controller
    {
        private readonly PassportDbContext _context;

        private readonly UserManager<OAUser> _userManager;
        private readonly SignInManager<OAUser> _signInManager;

        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(PassportDbContext context, IJwtTokenService jwtTokenService, UserManager<OAUser> userManager, SignInManager<OAUser> signInManager)
        {
            _context = context;

            _userManager = userManager;
            _signInManager = signInManager;

            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public async Task<string> UserProfile(string Token, string AppId)
        {
            var decodedData = _jwtTokenService.ValidateToken(Token, AppId);
            if (decodedData != null)
            {
                var user = await _userManager.FindByIdAsync(decodedData.Payload.FirstOrDefault(p => p.Key == "ForUser").Value.ToString());
                if (user == null)
                {
                    return "User not found";
                }

                var app = _context.Apps.FirstOrDefault(a => a.AppId == AppId);
                if (app != null)
                {
                    if (app.GetUsersAuthorized().Contains(user.Id))
                    {
                        return Base64Tool.Encode(JsonSerializer.Serialize(new
                        {
                            user.Id,
                            user.Email,
                            user.UserName,
                            user.PhoneNumber
                        }));
                    }

                    return "User not authorized";
                }

                return "App not found";
            }

            return "Invalid token";
        }

        [HttpGet]
        public IActionResult SignUp(string AppId, string RedirectUri)
        {
            return View(new SignUpModel { AppId = AppId, RedirectUri = RedirectUri });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new OAUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    CreateTime = DateTime.UtcNow,
                    // EmailConfirmed = false,
                    // PhoneNumberConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SignIn", "Auth", new { model.AppId, model.RedirectUri });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> My()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var authorizedApps = new List<App>();
                    var appsCreatedByUser = new List<App>();
                    foreach(var app in _context.Apps)
                    {
                        if (app.GetUsersAuthorized().Contains(user.Id))
                        {
                            authorizedApps.Add(app);
                        }
                        if(app.ManagerId == user.Id)
                        {
                            appsCreatedByUser.Add(app);
                        }
                    }

                    return View(new MyAccountModel
                    {
                        User = user,
                        AuthorizedApps = authorizedApps,
                        AppsCreatedByUser = appsCreatedByUser
                    });
                }
            }

            return Unauthorized();
        }

        public IActionResult Login(string ReturnUrl)
        {
            return RedirectToAction("SignIn", "Auth");
        }
    }
}