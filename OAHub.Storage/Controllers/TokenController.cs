using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Data;
using OAHub.Storage.Models;
using OAHub.Storage.Models.ViewModels.Token;

namespace OAHub.Storage.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TokenController : Controller
    {
        private readonly StorageDbContext _context;

        public TokenController(StorageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult List()
        {
            var user = GetUserProfile();
            var list = new List<StorageApiToken>();
            user.GetOwnedApiTokens().ForEach(element =>
            {
                list.Add(_context.ApiTokens.FirstOrDefault(t => t.Id == element));
            });

            return View(new ListModel
            {
                OwnedTokens = list
            });
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddModel model)
        {
            var user = GetUserProfile();
            var tokens = user.GetOwnedApiTokens();
            var token = new StorageApiToken
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = model.Title,
                Description = model.Description,
                CreateTime = DateTime.UtcNow,
            };
            tokens.Add(token.Id);
            user.SetOwnedApiTokens(tokens);

            _context.ApiTokens.Add(token);
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Result), new { id = token.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Result(string id)
        {
            var user = GetUserProfile();
            var token = _context.ApiTokens.FirstOrDefault(t => t.Id == id);
            if (token != null && user.GetOwnedApiTokens().Contains(id))
            {
                token.TokenContent = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

                _context.ApiTokens.Update(token);
                await _context.SaveChangesAsync();

                return View(new ResultModel
                {
                    Token = token.TokenContent,
                    Id = token.Id
                });
            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = GetUserProfile();
            var token = _context.ApiTokens.FirstOrDefault(t => t.Id == id);
            if (token != null && user.GetOwnedApiTokens().Contains(id))
            {
                var tokens = user.GetOwnedApiTokens();
                tokens.Remove(id);
                user.SetOwnedApiTokens(tokens);

                _context.Users.Update(user);
                _context.ApiTokens.Remove(token);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(List));
            }

            return Unauthorized();
        }

        private StorageUser GetUserProfile()
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
    }
}