using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static CourseEnrollmentApp_API.Models.Pub;

namespace CourseEnrollmentApp_API.Services.Bug
{

    public class HttpResponseExceptionFilter(IErrorLogService errorLogService) : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;
        public IErrorLogService ErrorLogService { get; } = errorLogService;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                Task.Run(() => ErrorLogService.RegisterError(context.Exception));

                var msg = "An error happened. Please contact support";

                if (context.Exception.GetType() == typeof(InvalidDataException))
                {
                    msg = context.Exception.Message;
                }

                context.Result = new ObjectResult(new Dictionary<string, string> {
                    {
                            "error",
                            msg
                    },
                    {
                            "message",
                            context.Exception.Message
                    } })

                {
                    StatusCode = (int)StatusCode.InternalServerError
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
