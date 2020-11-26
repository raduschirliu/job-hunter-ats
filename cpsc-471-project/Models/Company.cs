using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    [Flags]
    public enum CompanySize
    {
        OneToTen = 0,
        ElevenToFifty = 1,
        FiftyOneToTwoFifty = 2,
        TwoFiftyOneToFiveHundred = 3,
        FiveHundredOneToOneThousand = 4,
        OneThousandOneToTwoThousand = 5,
        TwoThouandOneToFiveThousand = 6,
        FivethousandOneToTenThousand = 7,
        TenThousandOneToTwentyFiveThousand = 8,
        TwentyFiveThousandOneToFiftyThousand = 9,
        FiveThousandOneAndUp = 10
    }

    public class Company
    {
        [Key]
        public long CompanyId { get; set; }
        public CompanySize Size { get; set; }
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string Description { get; set; }
        [MaxLength(32)]
        public string Industry { get; set; }
        [Required]
        [ForeignKey("User")]
        public long AdminId { get; set; }

        // this is needed so that the foreign key constraint is automatically enforced
        // however, it will have a null value in the objects produced by the DB
        public User User { get; set; }
    }
    
    public class CompanyDTO
    {
        [Display(Name = "Company ID")]
        public long CompanyId { get; set; }

        [Display(Name = "Company Size")]
        public CompanySize Size { get; set; }

        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Industry")]
        public string Industry { get; set; }

        [Display(Name = "Admin Id")]
        public long AdminId { get; set; }
    }
}
