namespace CourseEnrollmentApp_API.Controllers
{
    public class CommonOutputModel
    {
        public List<Dictionary<string, string>> Errors { get; set; } = [];
        public void AddError(string key, string value)
        {
            Errors.Add(new Dictionary<string, string> { { key, value } });
        }
    }
}
