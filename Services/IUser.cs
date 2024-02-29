using CourseEnrollmentApp_API.Controllers.Auth;
using CourseEnrollmentApp_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseEnrollmentApp_API.Services
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateAsync(RegisterModelIn xUser);
        public Task<string> GetAuthToken(User xUser);
        public Task SeedRoles();
    }

    public class UserService : IUserService
    {
        public UserManager<User> UserManager { get; set; }
        public Setting.Setting Setting { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public DBContext Context { get; set; }

        public enum UserRoles
        {
            User, Admin
        }

        public UserService(UserManager<User> userMgr,
                           DBContext context,
                           Setting.Setting setting, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userMgr;
            Context = context;
            Setting = setting;
            RoleManager = roleManager;
        }
        public async Task SeedRoles()
        {
            const string Admin = "Admin";
            if (!RoleManager.RoleExistsAsync(Admin).Result)
            {
                IdentityRole role = new()
                {
                    Name = Admin
                };
                await RoleManager.CreateAsync(role);
            }

            const string User = "Student";
            if (!RoleManager.RoleExistsAsync(User).Result)
            {
                IdentityRole role = new()
                {
                    Name = User
                };
                await RoleManager.CreateAsync(role);
            }
        }
        public async Task<IdentityResult> CreateAsync(RegisterModelIn registerModelIn)
        {
            var user = new User
            {
                Email = registerModelIn.Email,
                UserName = registerModelIn.Email,
                Name = registerModelIn.Name,
            };
            IdentityResult result = await UserManager.CreateAsync(user, registerModelIn.Password);

            if (registerModelIn.Role > 0)
            {
                await UserManager.AddToRoleAsync(user, registerModelIn.Role.ToString());
            }

            await Context.Students.AddAsync(new Student { StudentId = user.Id });
            await Context.SaveChangesAsync();

            return result;
        }

        public async Task<string> GetAuthToken(User user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                    {
                        new(JwtRegisteredClaimNames.Jti, user.Id),
                        new(ClaimTypes.Email, user.Email)
                    };

            //Add the user roles in the claim so that the the role will be used for authorisation
            foreach (var role in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, role.ToUpper()));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.JwtSetting.IssuerSigningKey));

            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            //If production we set the token lifetime to 8 hours, otherwise the lifetime is some seconds
            /*0.000555556 is two second in unit of hours*/
            var expires = DateTime.Now.AddMonths(Setting.JwtSetting.TokenLifeTime);

            var securityToken = new JwtSecurityToken(Setting.JwtSetting.Issuer, Setting.JwtSetting.Audience, authClaims, null, expires, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);

        }

    }

}
