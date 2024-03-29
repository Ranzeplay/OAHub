﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models;
using OAHub.Base.Utils;
using OAHub.Passport.Data;
using OAHub.Passport.Models.ViewModels.Auth;
using OAHub.Passport.Services;

namespace OAHub.Passport.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<OAUser> _userManager;
        private readonly SignInManager<OAUser> _signInManager;

        private readonly PassportDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<OAUser> userManager, SignInManager<OAUser> signInManager, PassportDbContext context, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public IActionResult SignIn(string AppId, string RedirectUri)
        {
            return View(new SignInModel { AppId = AppId, RedirectUri = RedirectUri });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }

                        if (!string.IsNullOrEmpty(model.AppId))
                        {
                            return RedirectToAction(
                                nameof(Authorize),
                                new AuthorizeModel
                                {
                                    AppId = model.AppId,
                                    RedirectUri = model.RedirectUri,
                                    State = model.State
                                });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Errors occured while signing in");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authorize(AuthorizeModel model)
        {
            var user = await GetCurrentUserAsync();
            var app = _context.Apps.FirstOrDefault(a => a.AppId == model.AppId);
            if (app != null)
            {
                if (!app.GetUsersAuthorized().Contains(user.Id))
                {
                    return View(new AuthorizeConfirmModel
                    {
                        AppId = model.AppId,
                        AppName = app.Name,
                        Confirm = false,
                        RedirectUri = model.RedirectUri,
                        State = model.State
                    });
                }

                var code = GenerateCode(user);
                var url = new OAHubUrl
                {
                    UrlAddress = model.RedirectUri,
                    Params = new Dictionary<string, string>
                    {
                        { "code", code },
                        { "state", model.State }
                    }
                };
                return Redirect(url.ToStringUrl());
            }

            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Authorize(AuthorizeConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Confirm)
                {
                    var user = await GetCurrentUserAsync();
                    var app = _context.Apps.FirstOrDefault(a => a.AppId == model.AppId);
                    if (app != null)
                    {
                        var authorizedUsers = app.GetUsersAuthorized();
                        authorizedUsers.Add(user.Id);
                        app.SetUsersAuthorized(authorizedUsers);
                        _context.Apps.Update(app);
                        await _context.SaveChangesAsync();

                        var code = GenerateCode(user);
                        var url = new OAHubUrl
                        {
                            UrlAddress = model.RedirectUri,
                            Params = new Dictionary<string, string>
                            {
                                { "code", code },
                                { "state", model.State }
                            }
                        };
                        return Redirect(url.ToStringUrl());
                    }
                }
                else
                {
                    var url = new OAHubUrl
                    {
                        UrlAddress = model.RedirectUri,
                        Params = new Dictionary<string, string>
                            {
                                { "error", "user_denied" },
                                { "state", model.State }
                            }
                    };
                    return Redirect(url.ToStringUrl());
                }
            }

            return View(model);
        }

        [HttpGet]
        public string Token(string appid, string appsecret, string code)
        {
            var originalJsonCode = JsonSerializer.Deserialize<CodeModel>(Base64Tool.Decode(code));
            if (originalJsonCode.ExpireTime > DateTime.UtcNow)
            {
                var app = _context.Apps.FirstOrDefault(a => a.AppId == appid);
                if (app.AppSecret == appsecret)
                {
                    var token = _jwtTokenService.GenerateToken(appid, appsecret, originalJsonCode.ForUserId);

                    return token;
                }
            }

            return $"Code expired";
        }

        public async Task<IActionResult> SignOut(string RedirectUrl)
        {
            await _signInManager.SignOutAsync();

            if (RedirectUrl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(RedirectUrl);
        }

        public async Task<OAUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        public string GenerateCode(OAUser user)
        {
            return Base64Tool.Encode(JsonSerializer.Serialize(new CodeModel
            {
                ExpireTime = DateTime.UtcNow.AddMinutes(2),
                ForUserId = user.Id
            }));
        }
    }
}