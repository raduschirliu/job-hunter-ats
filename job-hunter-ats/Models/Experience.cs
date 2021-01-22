using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using job_hunter_ats.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace job_hunter_ats.Models
{
    public class Experience
    {

        [Required]
        [ForeignKey("Resume")]
        [Key, Column(Order = 0)]
        public long ResumeId { get; set; }
        public Resume Resume { get; set; } // for referential integrity

        [Required]
        [Key, Column(Order = 1)]
        public long Order { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

    }

    public class ExperienceDTO
    {
        [Display(Name = "Resume Id")]
        public long ResumeId { get; set; }

        [Display(Name = "Order")]
        public long Order { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}
