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
    public class Award
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
        
        public DateTime? DateReceived { get; set; }
        
        public string Value { get; set; }
        
    }
    public class AwardDTO
    {
        [Display(Name = "Resume Id")]
        public long ResumeId { get; set; }

        [Display(Name = "Order")]
        public long Order { get; set; }

        [Display(Name = "Award Name")]
        public string Name { get; set; }

        [Display(Name = "Date Received")]
        public DateTime? DateReceived { get; set; }

        [Display(Name = "value")]
        public string value { get; set; }

    }
}
