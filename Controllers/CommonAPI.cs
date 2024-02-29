using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Services.Bug;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CourseEnrollmentApp_API.Controllers
{
    public abstract class CommonAPI(UserManager<User> userMgr,
                   IErrorLogService errorLogService,
                   SignInManager<User> signinMgr = null,
                   DBContext context = null,
                   IConfiguration configuration = null,
                   Services.Setting.Setting setting = null) : ControllerBase
    {
        public UserManager<User> UserManager { get; set; } = userMgr;
        public IErrorLogService ErrorLogService { get; } = errorLogService;
        public SignInManager<User> SignInManager { get; set; } = signinMgr;
        public DBContext Context { get; set; } = context;
        public Dictionary<string, object> ReturnValue = [];

        public IConfiguration Configuration { get; } = configuration;
        public Services.Setting.Setting Setting { get; } = setting;

        public Task<User> AuthenticatedUser
        {
            get
            {
                return Task.Run(async () =>
                {
                    User user = null;
                    try
                    {
                        var claimsIdentity = User.Identity as ClaimsIdentity;
                        var userId = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        user = await UserManager.FindByIdAsync(userId);
                     
                        if (user == null)
                        {
                            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                            user = await UserManager.FindByEmailAsync(email);
                        }
                    }
                    catch { }

                    return user;
                });
            }
        }
    }
}
