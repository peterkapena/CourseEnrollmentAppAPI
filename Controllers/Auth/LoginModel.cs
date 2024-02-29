namespace CourseEnrollmentApp_API.Controllers.Auth
{
    public class LoginModelIn
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginModelOut : CommonOutputModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
    public class VerifyTokenModelOut : LoginModelOut
    {
        public string Role { get; set; }
    }
}
