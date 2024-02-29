using System;

namespace CourseEnrollmentApp_API.Models.TmpProc
{
    public class TmpTask
    {
        public long TmpTaskId { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public DateTime WhenRun { get; set; }
    }
}
