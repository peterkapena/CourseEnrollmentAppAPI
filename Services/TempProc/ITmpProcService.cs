using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Models.TmpProc;
using CourseEnrollmentApp_API.Services.Bug;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CourseEnrollmentApp_API.Services.TempProc
{
    public interface ITmpProcService
    {
        public Task<bool> HasAlreadyRun(MethodInfo method);
        public Task SaveTmpTask(MethodInfo method);
        public Task<ITmpProcService> Run();

    }

    public partial class TmpProc(DBContext context, IErrorLogService errorLogService, IUserService userService, IConfiguration configuration) : ITmpProcService
    {

        #region "constructor and properties"

        public DBContext Context { get; } = context;
        public IErrorLogService ErrorLogService { get; } = errorLogService;
        public IUserService UserService { get; } = userService;
        public IConfiguration Configuration { get; } = configuration;
        #endregion

        public async Task<ITmpProcService> Run()
        {
            foreach (var method in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (method.GetCustomAttributes(typeof(TmpProcAtt), false).Length > 0 && !(await HasAlreadyRun(method)))
                {
                    Func<Task> runStep = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), this, method);
                    try
                    {
                        await runStep(); // Awaiting the asynchronous operation
                        await SaveTmpTask(method); // Assuming SaveTmpTask is an async method
                    }
                    catch (Exception ex)
                    {
                        await ErrorLogService.RegisterError(new Exception($"{method.Name} Failed to run", ex));
                    }
                }
            }
            return this;
        }

        public async Task<bool> HasAlreadyRun(MethodInfo method)
        {
            try
            {
                return await Context.TmpTasks
                    .Where(t => t.MethodName == method.Name && t.ClassName == method.GetCustomAttribute<TmpProcAtt>().ClassName)
                    .AnyAsync();
            }
            catch (Exception ex)
            {
                await ErrorLogService.RegisterError(new Exception("Temp proc - HasAlreadyRun", ex));
                return false;
            }
        }

        public async Task SaveTmpTask(MethodInfo method)
        {
            var tmpProcAtt = method.GetCustomAttribute<TmpProcAtt>();
            var tmpProc = new TmpTask { ClassName = tmpProcAtt.ClassName, MethodName = method.Name, WhenRun = DateTime.Now };
            await Context.TmpTasks.AddAsync(tmpProc);
            await Context.SaveChangesAsync();
        }
    }
}
