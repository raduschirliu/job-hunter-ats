using cpsc_471_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cpsc_471_project.Authentication
{
    public class RegistrationResponse
    {
        public UserDTO User { get; set; }
        public AuthResponse Auth { get; set; }
    }
}
