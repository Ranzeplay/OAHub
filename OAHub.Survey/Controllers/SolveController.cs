using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OAHub.Survey.Data;
using OAHub.Survey.Models.ViewModels.Solve;

namespace OAHub.Survey.Controllers
{
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

        public IActionResult Submit(string formId)
        {
            return BadRequest();
        }
    }
}