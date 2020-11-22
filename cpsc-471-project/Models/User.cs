using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }
    }

    public class UserDTO
    {
        [Display(Name = "User ID")]
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
