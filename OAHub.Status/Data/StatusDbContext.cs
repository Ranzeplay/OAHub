using Microsoft.EntityFrameworkCore;
using OAHub.Status.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Data
{
    public class StatusDbContext : DbContext
    {
        public StatusDbContext(DbContextOptions<StatusDbContext> options) : base(options)
        {
        }

        public DbSet<StatusUser> Users { get; set; }
    }
}
