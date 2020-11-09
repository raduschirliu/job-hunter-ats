using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    [Flags]
    public enum UserRole
    {
        Admin = 0b_0000_0001,
        Recruiter = 0b_0000_0010,
        Candidate = 0b_0000_0100
    }
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        [Display(Name = "{Phone Number")]
        [MaxLength(32)]
        public string Phone { get; set; }

        /*
        [Required]
        private string Salt { get; set; }

        [Required]
        private string Hash { get; set; }
        */

        public static List<User> SampleData()
        {
            List<User> returnedUsers = new List<User>();

            User testUser1 = new User()
            {
                UserId = 1,
                Role = UserRole.Admin,
                FirstName = "Bob",
                LastName = "Smith",
                Phone = "555-555-5555",
            };

            returnedUsers.Add(testUser1);

            return returnedUsers;
        }
    }
}
