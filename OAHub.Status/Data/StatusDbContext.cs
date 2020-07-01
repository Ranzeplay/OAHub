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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusUser>().HasMany(u => u.Tracks).WithOne(t => t.CreatedBy).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Track>().HasMany(t => t.Posts).WithOne(p => p.ForTrack).OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<StatusUser> Users { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
