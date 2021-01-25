using job_hunter_ats.Models;
namespace job_hunter_ats.Authentication
{
    public class UserAuthResponse
    {
        public UserDTO User { get; set; }
        public AuthResponse Auth { get; set; }
    }
}
