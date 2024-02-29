using CourseEnrollmentApp_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static CourseEnrollmentApp_API.Models.Pub;

namespace CourseEnrollmentApp_API.Services.Bug
{
    public interface IErrorLogService
    {
        public Task RegisterError(Exception ex, string userId = null);
    }
    public class ErrorLogService(DBContext context) : IErrorLogService
    {
        public DBContext Context { get; } = context;

        public async Task RegisterError(Exception ex, string userId = null)
        {
            foreach (EntityEntry entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }

            var error = new ErrorLog
            {
                UserId = userId,
                StackTrace = ex.StackTrace,
                HelpLink = ex.HelpLink,
                HResult = ex.HResult,
                InnerException = ex.InnerException?.ToString() ?? ex.StackTrace,
                Message = ex.Message,
                Source = ex.Source,
                Date = DateTime.Now
            };

            await Context.Errors.AddAsync(error);
            await Context.SaveChangesAsync();
            foreach (EntityEntry entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }
    }
}
