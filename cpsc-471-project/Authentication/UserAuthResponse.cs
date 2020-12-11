using cpsc_471_project.Models;
namespace cpsc_471_project.Authentication
{
    public class UserAuthResponse
    {
        public UserDTO User { get; set; }
        public AuthResponse Auth { get; set; }
    }
}
