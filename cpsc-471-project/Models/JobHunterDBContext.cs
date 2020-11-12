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
    }
}
