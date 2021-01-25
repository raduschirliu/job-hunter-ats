using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace job_hunter_ats.Models
{
    public class Referral
    {
        [ForeignKey("Application")]
        [Key, Column(Order = 0)]
        public long ApplicationId { get; set; }
        public Application Application { get; set; } // Enforces referential integrity

        [Key, Column(Order = 1)]
        public long ReferralId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Position { get; set; }

        [MaxLength(255)]
        public string Company { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }
    }

    public class ReferralDTO
    {
        [Display(Name = "Application ID")]
        public long ApplicationId { get; set; }

        [Display(Name = "Referral ID")]
        public long ReferralId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }
    }
}
