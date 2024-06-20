using Core.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Data
{
    public class HangfireDbContext : DbContext
    {
        public DbSet<BackupSettings> BackupSettings { get; set; }

        public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
    
}
