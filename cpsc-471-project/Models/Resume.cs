using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public class Resume
    {
        [Key]
        public long ResumeId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Candidate")]
        public long CandidateId { get; set; }

        // Used to enforce referential integrity constraints
        public User Candidate { get; set; }
    }

    public class ResumeDTO
    {
        public long ResumeId { get; set; }
        public string Name { get; set; }
        public long CandidateId { get; set; }
    }
}
