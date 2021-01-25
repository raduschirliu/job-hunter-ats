using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace job_hunter_ats.Models
{
    public class Interview
    {
        [Key]
        [ForeignKey("Application")]
        public long ApplicationId { get; set; }

        public Application Application { get; set; }
        
        [Required]
        [ForeignKey("Recruiter")]
        public string RecruiterId { get; set; }
        
        public User Recruiter { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    public class InterviewDTO
    {
        public long ApplicationId { get; set; }
        public string RecruiterId { get; set; }
        public DateTime Date { get; set; }
    }
}
