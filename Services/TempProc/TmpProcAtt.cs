using System;

namespace CourseEnrollmentApp_API.Services.TempProc
{
    public class TmpProcAtt : Attribute
    {
        public TmpProcAtt(string className)
        {
            ClassName = className;
        }

        public string ClassName { get; }
    }
}
