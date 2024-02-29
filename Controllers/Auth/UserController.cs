using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Services;
using CourseEnrollmentApp_API.Services.Bug;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseEnrollmentApp_API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserManager<User> userMgr,
                          DBContext context,
                          Services.Setting.Setting setting,
                           IUserService userService,
                          IErrorLogService errorLogService) : CommonAPI(userMgr: userMgr,
            context: context,
            setting: setting,
             errorLogService: errorLogService)
    {
        public IUserService UserService { get; } = userService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelIn loginModelIn)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(loginModelIn.Email);
                LoginModelOut loginModelOut = new();

                if (user is not null && await UserManager.CheckPasswordAsync(user, loginModelIn.Password))
                {
                    string token = await UserService.GetAuthToken(user);
                    loginModelOut.Token = token;
                    loginModelOut.Email = user.Email;
                    loginModelOut.UserId = user.Id;
                }
                else
                {
                    loginModelOut.AddError("login", "login failed");
                }
                return Ok(loginModelOut);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelIn registerModelIn)
        {
            var user = await UserManager.FindByEmailAsync(registerModelIn.Email);
            CommonOutputModel registerModelOut = new();

            if (user is null)
            {
                var rslt = await UserService.CreateAsync(registerModelIn);
                if (!rslt.Succeeded)
                    foreach (var error in rslt.Errors)
                    {
                        registerModelOut.AddError(error.Code, error.Description);
                    }
                Ok(registerModelOut);
            }
            else
            {
                registerModelOut.AddError("Duplicate", "A user with this email already exists.");
            }
            return Ok(registerModelOut);
        }

        [HttpPost("verify_tkn")]
        public async Task<IActionResult> VerifyToken()
        {

            var user = await AuthenticatedUser;
            if (user is null)
            {
                var rtn = new LoginModelOut();
                rtn.AddError("Token", "Invalid token");
                return Ok(rtn);
            }
            else
            {
                string token = await UserService.GetAuthToken(user);
                var claimsIdentity = User.Identity as ClaimsIdentity;
                return Ok(new VerifyTokenModelOut
                {
                    Email = user.Email,
                    Token = token,
                    Role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value
                });
            }
        }
    }
}
