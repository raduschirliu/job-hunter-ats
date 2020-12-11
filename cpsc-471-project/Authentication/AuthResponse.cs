using System;
using System.Collections.Generic;

namespace cpsc_471_project.Authentication
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
}
