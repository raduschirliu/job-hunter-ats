using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cpsc_471_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using cpsc_471_project.Authentication;

namespace cpsc_471_project.Models
{
    public class JobHunterDBContext : IdentityDbContext<User>
    {
        public JobHunterDBContext(DbContextOptions<JobHunterDBContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Education> Education { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Experience> Experiences { get; set; }

        public DbSet<Award> Awards { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<JobPost> JobPosts { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Resume> Resumes { get; set; }

        public DbSet<Referral> Referrals { get; set; }

        public DbSet<Recruiter> Recruiters { get; set; }

        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Recruiter>().HasKey(x => new { x.UserId, x.CompanyId });
            modelBuilder.Entity<Referral>().HasKey(x => new { x.ApplicationId, x.ReferralId });
            modelBuilder.Entity<Offer>().HasKey(x => new { x.ApplicationId, x.OfferId });
            modelBuilder.Entity<Award>().HasKey(x => new { x.ResumeId, x.Order });
            modelBuilder.Entity<Skill>().HasKey(x => new { x.ResumeId, x.Order });
            modelBuilder.Entity<Certification>().HasKey(x => new { x.ResumeId, x.Order });
            modelBuilder.Entity<Experience>().HasKey(x => new { x.ResumeId, x.Order });
            modelBuilder.Entity<Education>().HasKey(x => new { x.ResumeId, x.Order });
            modelBuilder.Entity<Project>().HasKey(x => new { x.ResumeId, x.Order });
        }
    }
}
