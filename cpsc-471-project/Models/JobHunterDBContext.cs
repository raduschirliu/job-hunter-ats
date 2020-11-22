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
    public class JobHunterDBContext : IdentityDbContext<AppUser>
    {
        public JobHunterDBContext(DbContextOptions<JobHunterDBContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        public DbSet<Company> Company { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
