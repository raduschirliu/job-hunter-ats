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
    public class Project
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
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }

    public class ProjectDTO
    {
        [Display(Name = "Resume Id")]
        public long ResumeId { get; set; }
        
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Order")]
        public long Order { get; set; }
    }
}
