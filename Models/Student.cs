using System.Text.Json.Serialization;

namespace CourseEnrollmentApp_API.Models
{
    public class Student
    {
        public string StudentId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
       public ICollection<StudentCourseEnrollmentApp_API> StudentCourseEnrollmentApp_APIs { get; set; }
    }
}
