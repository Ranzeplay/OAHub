using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models.SurveyModels;
using OAHub.Base.Models.SurveyModels.Forms.Standard;
using OAHub.Survey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Survey.Data
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
        {
        }

        public DbSet<SurveyUser> Users { get; set; }

        public DbSet<SurveyOrganization> SurveyOrganizations { get; set; } 

        public DbSet<StandardForm> StandardForms { get; set; }

        public DbSet<StandardAnswer> StandardAnswers { get; set; }
    }
}
