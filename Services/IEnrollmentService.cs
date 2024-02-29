namespace CourseEnrollmentApp_API.Services
{
    interface ICourseEnrollmentApp_APIService
    {
        Task Enroll();
        Task Deregister();
    }
    public class CourseEnrollmentApp_APIService : ICourseEnrollmentApp_APIService
    {
        public Task Deregister()
        {
            throw new NotImplementedException();
        }

        public Task Enroll()
        {
            throw new NotImplementedException();
        }
    }
}
