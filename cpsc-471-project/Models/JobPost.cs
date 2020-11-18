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

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public int Salary { get; set; }

        public DateTime ClosingDate { get; set; }

        [ForeignKey("User")]
        public long RecruiterId { get; set; }
    }
}
