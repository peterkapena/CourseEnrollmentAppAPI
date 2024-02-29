using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollmentApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(UserManager<User> userMgr, IErrorLogService errorLogService, DBContext context)
        : CommonAPI(userMgr, errorLogService, context: context)
    {
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> AddCourse(Course course)
        {
            Context.Courses.Add(course);
            await Context.SaveChangesAsync();

            return Ok(new { course.Title, course.Description, course.Id });
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteCourse(long Id)
        {
            var course = await Context.Courses.Where(c => c.Id == Id).SingleOrDefaultAsync();
            Context.Courses.Remove(course);
            await Context.SaveChangesAsync();

            return Ok(new { course.Title, course.Description, course.Id });
        }

        [HttpGet("{id:long?}")]
        [Authorize]
        public async Task<ActionResult> GetCourses(long Id)
        {
            if (Id == 0)
            {
                var sId = (await AuthenticatedUser).Id;
                var unenrolledCourses = Context.Courses.Where(c => !Context.StudentCourseEnrollmentApp_APIs.Any(se => se.StudentId == sId && se.CourseId == c.Id));
                var courses = await unenrolledCourses.Select(c => new { c.Id, c.Description, c.Title }).AsNoTracking().ToListAsync();

                return Ok(courses);
            }

            var course = await Context.Courses.Where(c => c.Id == Id).SingleOrDefaultAsync();

            return Ok(new { course.Title, course.Description, course.Id });
        }
    }
}
