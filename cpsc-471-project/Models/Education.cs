
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace cpsc_471_project.Models
{
    public class Education
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
        public string SchoolName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string Major { get; set; }
    }

    public class EducationDTO
    {
        [Display(Name = "Resume Id")]
        public long ResumeId { get; set; }

        [Display(Name = "Order")]
        public long Order { get; set; }

        [Display(Name = " School Name")]
        public string SchoolName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Major")]
        public string Major { get; set; }
    }
}
