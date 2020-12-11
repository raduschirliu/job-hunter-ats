using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public class Recruiter
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; } // Required for referential integrity constraints

        [Key, Column(Order = 1)]
        [ForeignKey("Company")]
        public long CompanyId { get; set; }
        public Company Company { get; set; } // Required for referential integrity constraints
    }

    public class RecruiterDTO
    {
        public UserDTO User { get; set; }
        public CompanyDTO Company { get; set; }
    }

    public class SimplifiedRecruiterDTO
    {
        public string UserId { get; set; }
        public long CompanyId { get; set; }
    }
}
