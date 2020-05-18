using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Survey.Areas.CreateSurvey.Models.ViewModels.Standard
{
    public class UpdateSettingsModel
    {
        public string Title { get; set; }

        public bool AllowAnonymous { get; set; }

        public bool AllowMultipleSubmits { get; set; }
    }
}
