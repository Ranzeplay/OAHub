using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Passport.Data
{
    public class PassportDbContext : IdentityDbContext<OAUser>
    {
        public PassportDbContext(DbContextOptions<PassportDbContext> options) : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }

        public DbSet<Log> Logs { get; set; }
    }
}
