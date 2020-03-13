using Microsoft.EntityFrameworkCore;
using OAHub.Answers.Models;
using OAHub.Base.Models.AnswersModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Answers.Data
{
    public class AnswersDbContext : DbContext
    {
        public AnswersDbContext(DbContextOptions<AnswersDbContext> options) : base(options)
        {
        }

        public DbSet<AnswersUser> Users { get; set; }

        public DbSet<AnswersOrganization> AnswersOrganizations { get; set; } 
    }
}
