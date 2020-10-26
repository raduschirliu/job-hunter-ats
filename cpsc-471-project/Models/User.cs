using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Models
{
    [Flags]
    public enum Role
    {
        Admin = 0b_0000_0001,
        Recruiter = 0b_0000_0010,
        Candidate = 0b_0000_0100
    }
    public class User
    {
        public long id { get; set; }
        public Role role { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        private string salt { get; set; }
        private string hash { get; set; }
    }
}
