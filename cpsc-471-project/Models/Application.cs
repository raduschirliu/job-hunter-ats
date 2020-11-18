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
        Rejected //:(
    }

    public class Application
    {
        [Key]
        public long ApplicationId { get; set; }

        [Required]
        [ForeignKey("JobPost")] //? "Company" or "CompanyId"?
        public long JobId { get; set; }

        public DateTime DateSubmitted { get; set; }

        [Required]
        public Status Status { get; set; }

        public string CoverLetter { get; set; }

        [ForeignKey("Resume")]
        public long ResumeId { get; set; }
    }
}
