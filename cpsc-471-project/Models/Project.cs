using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cpsc_471_project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace cpsc_471_project.Models
{
    public class Project
    {

        [Required]
        [ForeignKey("Resume")]
        [Key, Column(Order = 0)]
        public long ResumeId { get; set; }
        [Key, Column(Order = 1)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Key, Column(Order = 2)]
        public long Order { get; set; }

        public Resume Resume { get; set; }
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
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Order")]
        public long Order { get; set; }
    }
}
