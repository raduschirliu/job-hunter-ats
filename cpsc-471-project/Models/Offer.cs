using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public class Offer
    {
        [ForeignKey("Application")]
        [Key, Column(Order = 0)]
        public long ApplicationId { get; set; }
        public Application Application { get; set; } // Enforces referential integrity

        [Key, Column(Order = 1)]
        public long OfferId { get; set; }

        public DateTime AcceptanceEndDate { get; set; }

        [MaxLength(1024)]
        public string Text { get; set; }
    }

    public class OfferDTO
    {
        [Display(Name = "Application ID")]
        public long ApplicationId { get; set; }

        [Display(Name = "Offer ID")]
        public long OfferId { get; set; }

        [Display(Name = "Acceptance End Date")]
        public DateTime AcceptanceEndDate { get; set; }

        [Display(Name = "Text")]
        public string Text { get; set; }
    }
}
