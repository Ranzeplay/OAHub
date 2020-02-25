using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models.Extensions;
using OAHub.Organization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Organization.Data
{
    public class OrganizationDbContext : DbContext
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : base(options)
        {
        }

        public DbSet<OrganizationUser> Users { get; set; }

        public DbSet<Base.Models.OrganizationModels.Organization> Organizations { get; set; }

        public DbSet<Extension> Extensions { get; set; }
    }
}
