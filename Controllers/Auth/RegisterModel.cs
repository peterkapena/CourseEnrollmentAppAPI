using CourseEnrollmentApp_API.Models;

namespace CourseEnrollmentApp_API.Controllers.Auth
{
    public class RegisterModelIn
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Roles Role { get; set; }
    }
}
