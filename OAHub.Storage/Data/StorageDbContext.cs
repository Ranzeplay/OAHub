using Microsoft.EntityFrameworkCore;
using OAHub.Base.Models.StorageModels;
using OAHub.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Storage.Data
{
    public class StorageDbContext : DbContext
    {
        public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
        {
        }

        public DbSet<StorageUser> Users { get; set; }

        public DbSet<StorageApiToken> ApiTokens { get; set; }

        // Storage Models
        public DbSet<Shelf> Shelves { get; set; }

        public DbSet<Case> Cases { get; set; }

        public DbSet<Item> Items { get; set; }
    }
}
