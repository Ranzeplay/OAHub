using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAHub.Base.Models.SurveyModels.Forms.Answers;
using OAHub.Base.Models.SurveyModels.Forms.Questions;
using OAHub.Base.Models.SurveyModels.Forms.Standard;
using OAHub.Base.Utils;
using OAHub.Survey.Data;
using OAHub.Survey.Models;
using OAHub.Survey.Models.ViewModels.Solve;

namespace OAHub.Survey.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SolveController : Controller
    {
        private readonly SurveyDbContext _context;

        public SolveController(SurveyDbContext context)
        {
            _context = context;
        }

        public IActionResult Do(string formId)
        {
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                if (form.IsPublished)
                {
                    var model = new DoModel
                    {
                        FormId = form.Id,
                        Title = form.Title,
                        Questions = form.GetContent()
                    };

                    return View(model);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string formId, string encodedData)
        {
            var user = GetUserProfile();
            var form = _context.StandardForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                if (form.IsPublished)
                {
                    var answer = new StandardAnswer
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ForFormId = form.Id,
                        Submitter = user.Id,
                        SubmitTime = DateTime.UtcNow
                    };

                    var formContent = form.GetContent();
                    var decodedData = JsonSerializer.Deserialize<List<string>>(Base64Tool.Decode(encodedData));

                    var answerContent = answer.GetContent();
                    foreach (var item in decodedData)
                    {
                        var splited = Base64Tool.Decode(item).Split('-');

                        // We need to get QuestionId, QuestionId is in Questions.Body, and all questions are implemented Question.Body
                        var question = formContent.FirstOrDefault(q => (JsonSerializer.Deserialize<Base.Models.SurveyModels.Forms.Questions.Body>(q.Value.ToString())).QuestionId == splited[0]);

                        switch (question.Key)
                        {
                            case QuestionType.SingleSelect:
                                var singleSelect = new Base.Models.SurveyModels.Forms.Answers.Select
                                {
                                    QuestionId = JsonSerializer.Deserialize<Base.Models.SurveyModels.Forms.Questions.Body>(question.Value.ToString()).QuestionId,
                                    SelectedIndex = int.Parse(splited[1])
                                };
                                answerContent.Add(new KeyValuePair<QuestionType, object>(QuestionType.SingleSelect, singleSelect));
                                break;
                            case QuestionType.MultiSelect:
                                var multiSelect = new Base.Models.SurveyModels.Forms.Answers.MultiSelect
                                {
                                    QuestionId = JsonSerializer.Deserialize<Base.Models.SurveyModels.Forms.Questions.Body>(question.Value.ToString()).QuestionId,
                                    SelectedIndexes = new List<int>()
                                };
                                var selected = JsonSerializer.Deserialize<List<string>>(Base64Tool.Decode(splited[1]));
                                foreach (var index in selected)
                                {
                                    multiSelect.SelectedIndexes.Add(int.Parse(index));
                                }
                                answerContent.Add(new KeyValuePair<QuestionType, object>(QuestionType.MultiSelect, multiSelect));
                                break;
                            case QuestionType.BlankFill:
                                var blankFill = new Base.Models.SurveyModels.Forms.Answers.BlankFill
                                {
                                    QuestionId = JsonSerializer.Deserialize<Base.Models.SurveyModels.Forms.Questions.Body>(question.Value.ToString()).QuestionId,
                                    Text = Base64Tool.Decode(splited[1])
                                };
                                answerContent.Add(new KeyValuePair<QuestionType, object>(QuestionType.BlankFill, blankFill));
                                break;
                            case QuestionType.Check:
                                var check = new Base.Models.SurveyModels.Forms.Answers.Check
                                {
                                    QuestionId = JsonSerializer.Deserialize<Base.Models.SurveyModels.Forms.Questions.Body>(question.Value.ToString()).QuestionId,
                                    IsChecked = bool.Parse(splited[1])
                                };
                                answerContent.Add(new KeyValuePair<QuestionType, object>(QuestionType.Check, check));
                                break;
                            default:
                                break;
                        }
                    }

                    answer.SetContent(answerContent);
                    _context.StandardAnswers.Add(answer);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Complete));
                }
            }

            return NotFound();
        }

        public IActionResult Complete()
        {
            return View();
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