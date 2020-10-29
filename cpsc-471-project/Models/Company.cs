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
        
        [Display(Name = "Company Size")]
        public CompanySize Size { get; set; }
        
        [Display(Name = "Company Name")]
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(1024)]
        public string Description { get; set; }

        [Display(Name = "Industry")]
        [MaxLength(32)]
        public string Industry { get; set; }

        [Display(Name = "Admin Id")]
        [Required]
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        [Required]
        public User User { get; set; }
    }
}
