using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public class JobPost
    {
        [Key]
        public long JobPostId { get; set; }

        [Required]
        [ForeignKey("Company")] //? "Company" or "CompanyId"?
        public long CompanyId { get; set; }
        public Company Company { get; set; } // for referential integrity 

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public int Salary { get; set; }

        public DateTime ClosingDate { get; set; }

        [ForeignKey("User")]
        public long RecruiterId { get; set; }
        public User User { get; set; } // for referential integrity 
    }

    public class JobPostDTO
    {
        [Display(Name = "Job Post ID")]
        public long JobPostId { get; set; }

        [Display(Name = "Company ID")]
        public long CompanyId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Salary")]
        public int Salary { get; set; }

        [Display(Name = "Closing Date")]
        public DateTime ClosingDate { get; set; }

        [Display(Name = "Recruiter ID")]
        public long RecruiterId { get; set; }
    }
}
