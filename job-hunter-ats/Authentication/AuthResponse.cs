using System;
using System.Collections.Generic;

namespace job_hunter_ats.Authentication
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
}
