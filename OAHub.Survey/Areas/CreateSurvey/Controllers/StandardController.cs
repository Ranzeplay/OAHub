using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.SurveyModel.Forms;
using OAHub.Base.Models.SurveyModel.Forms.Questions;
using OAHub.Base.Utils;
using OAHub.Survey.Areas.CreateSurvey.Models.ViewModels.Standard;
using OAHub.Survey.Data;
using OAHub.Survey.Models;

namespace OAHub.Survey.Areas.CreateSurvey.Controllers
{
    [Area("CreateSurvey")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class StandardController : Controller
    {
        private readonly SurveyDbContext _context;

        public StandardController(SurveyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Summary(string formId)
        {
            var user = GetUserProfile();
            StandardForm form = null;

            if (formId == null)
            {
                form = new StandardForm
                {
                    Id = Guid.NewGuid().ToString("N"),
                    AuthorId = user.Id,
                    CreateTime = DateTime.Now
                };

                _context.StandardForms.Add(form);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Summary), new { formId = form.Id });
            }

            form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                return View(new SummaryModel
                {
                    Id = form.Id,
                    Title = form.Title,
                    AllowAnonymous = form.AllowAnonymous,
                    AllowMultipleSubmits = form.AllowMultipleSubmits,
                    Content = form.GetContent(),
                    CreateTime = form.CreateTime,
                    Deadline = form.Deadline
                });
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBasicSettings(string formId, UpdateSettingsModel model)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                form.Title = model.Title;
                form.AllowAnonymous = model.AllowAnonymous;
                form.AllowMultipleSubmits = model.AllowMultipleSubmits;
                _context.StandardForms.Update(form);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Summary), new { formId = form.Id });
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Select(string formId)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                return View();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Select(string formId, string description, string encodedData)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                var model = new Select
                {
                    QuestionId = Guid.NewGuid().ToString("N"),
                    Description = description,
                    Selections = JsonSerializer.Deserialize<List<string>>(Base64Tool.Decode(encodedData))
                };

                var content = form.GetContent();
                content.Add(new KeyValuePair<Base.Models.SurveyModel.Forms.Questions.Type, object>(Base.Models.SurveyModel.Forms.Questions.Type.SingleSelect, model));
                form.SetContent(content);
                _context.StandardForms.Update(form);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Summary), new { formId = form.Id });
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult MultiSelect(string formId)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                return View();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> MultiSelect(string formId, string description, string encodedData)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                var model = new MultiSelect
                {
                    QuestionId = Guid.NewGuid().ToString("N"),
                    Description = description,
                    Selections = JsonSerializer.Deserialize<List<string>>(Base64Tool.Decode(encodedData))
                };

                var content = form.GetContent();
                content.Add(new KeyValuePair<Base.Models.SurveyModel.Forms.Questions.Type, object>(Base.Models.SurveyModel.Forms.Questions.Type.MultiSelect, model));
                form.SetContent(content);
                _context.StandardForms.Update(form);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Summary), new { formId = form.Id });
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Check(string formId)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                return View();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Check(string formId, string description)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                var model = new Check
                {
                    QuestionId = Guid.NewGuid().ToString("N"),
                    Description = description,
                };

                var content = form.GetContent();
                content.Add(new KeyValuePair<Base.Models.SurveyModel.Forms.Questions.Type, object>(Base.Models.SurveyModel.Forms.Questions.Type.Check, model));
                form.SetContent(content);
                _context.StandardForms.Update(form);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Summary), new { formId = form.Id });
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