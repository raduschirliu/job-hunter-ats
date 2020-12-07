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
        public string CandidateId { get; set; }

        // Used to enforce referential integrity constraints
        public User Candidate { get; set; }
    }

    public class ResumeDTO
    {
        public long ResumeId { get; set; }
        public string Name { get; set; }
        public string CandidateId { get; set; }
    }

    public class ResumeDetailDTO
    {
        public long ResumeId { get; set; }
        public string Name { get; set; }
        public UserDTO Candidate { get; set; }
        public List<AwardDTO> Awards { get; set; }
        public List<CertificationDTO> Certifications { get; set; }
        public List<EducationDTO> Education { get; set; }
        public List<ExperienceDTO> Experience { get; set; }
        public List<ProjectDTO> Projects { get; set; }
        public List<SkillDTO> Skills { get; set; }
    }
}
