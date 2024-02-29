using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseEnrollmentApp_API.Models
{
    public class CommonModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
    public enum Roles
    {
        ADMIN = 1,
        STUDENT
    }
}
