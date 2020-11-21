using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public enum Status
    {
        Sent,
        InReview,
        Accepted,
        Rejected
    }

    public class Application
    {
        [Key]
        public long ApplicationId { get; set; }

        [Required]
        [ForeignKey("JobPost")] 
        public long JobId { get; set; }
        public JobPost JobPost { get; set; }

        [Required]
        public DateTime DateSubmitted { get; set; }

        [Required]
        public Status Status { get; set; }

        public string CoverLetter { get; set; }

        //[ForeignKey("Resume")]
        //public long ResumeId { get; set; }
        //public Resume Resume { get; set; } 
    }

    public class ApplicationDTO
    {
        [Display(Name = "Application ID")]
        public long ApplicationId { get; set; }

        [Display(Name = "Job ID")]
        public long JobId { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Cover Letter")]
        public string CoverLetter { get; set; }

        //[Display(Name = "Resume ID")]
        //public long ResumeId { get; set; }
    }
}
