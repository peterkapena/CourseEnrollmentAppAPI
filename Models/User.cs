using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseEnrollmentApp_API.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Student Student { get; set; }
        [NotMapped]
        public string PassWord { get; set; }
    }
}
