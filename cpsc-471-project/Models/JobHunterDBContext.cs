using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cpsc_471_project.Models;

namespace cpsc_471_project.Models
{
    public class JobHunterDBContext : DbContext
    {
        public JobHunterDBContext(DbContextOptions<JobHunterDBContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        public DbSet<Company> Company { get; set; }

        public DbSet<Resume> Resume { get; set; }

        public DbSet<Certification> Certification { get; set; }

        public DbSet<Education> Education { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<Award> Award { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Award>().HasKey(x => new { x.ResumeId, x.Name });
            modelBuilder.Entity<Skill>().HasKey(x => new { x.ResumeId, x.Name });
            modelBuilder.Entity<Certification>().HasKey(x => new { x.ResumeId, x.Name });
            modelBuilder.Entity<Experience>().HasKey(x => new { x.ResumeId, x.Company });
            modelBuilder.Entity<Education>().HasKey(x => new { x.ResumeId, x.Name });

        }
    }
}
