using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CourseEnrollmentApp_API.Models
{
    public class Course : CommonModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [JsonIgnore] public ICollection<StudentCourseEnrollmentApp_API> StudentCourseEnrollmentApp_APIs { get; set; }
    }
}
