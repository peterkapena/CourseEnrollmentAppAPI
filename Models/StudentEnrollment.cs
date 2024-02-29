namespace CourseEnrollmentApp_API.Models
{
    public class StudentCourseEnrollmentApp_API : CommonModel
    {
        public string StudentId { get; set; }
        public long CourseId { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
