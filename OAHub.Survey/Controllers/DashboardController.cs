using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Survey.Data;
using OAHub.Survey.Models;

namespace OAHub.Survey.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DashboardController : Controller
    {
        private readonly SurveyDbContext _context;

        public DashboardController(SurveyDbContext context)
        {
            _context = context;
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult AllSurveys()
        {
            var user = GetUserProfile();
            if (user != null)
            {
                var list = _context.StandardForms.Where(f => f.AuthorId == user.Id).ToList();

                return View(list);
            }

            return NotFound();
        }

        private SurveyUser GetUserProfile()
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