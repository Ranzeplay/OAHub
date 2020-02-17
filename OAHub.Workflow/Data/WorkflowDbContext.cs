using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models.WorkflowModels;
using OAHub.Workflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Workflow.Data
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
        {
        }

        public DbSet<WorkflowUser> Users { get; set; }

        public DbSet<WorkflowOrganization> WorkflowOrganizations { get; set; }

        public DbSet<Base.Models.WorkflowModels.ProjectViewModel> Projects { get; set; }

        public DbSet<Job> Jobs { get; set; }
    }
}
