using CourseEnrollmentApp_API.Models;
using CourseEnrollmentApp_API.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CourseEnrollmentApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentEnrollmentController(UserManager<User> userMgr, IErrorLogService errorLogService, DBContext context)
        : CommonAPI(userMgr, errorLogService, context: context)
    {
        [HttpPost("{studentId}/{courseId:long}")]
        [Authorize]
        public async Task<ActionResult> Enroll(string studentId, long courseId)
        {
            if (studentId.IsNullOrEmpty() || courseId <= 0)
            {
                return BadRequest("Invalid StudentId or CourseId.");
            }

            var CourseEnrollmentApp_API = await Context.StudentCourseEnrollmentApp_APIs.FindAsync(studentId, courseId);

            if (CourseEnrollmentApp_API is not null)
            {
                throw new ArgumentException("Already Enrolled");
            }

            bool studentExists = await Context.Students.AnyAsync(s => s.StudentId == studentId);
            bool courseExists = await Context.Courses.AnyAsync(c => c.Id == courseId);

            if (!studentExists || !courseExists)
            {
                return BadRequest("Invalid StudentId or CourseId.");
            }

            CourseEnrollmentApp_API = new StudentCourseEnrollmentApp_API { StudentId = studentId, CourseId = courseId };
            await Context.StudentCourseEnrollmentApp_APIs.AddAsync(CourseEnrollmentApp_API);
            await Context.SaveChangesAsync();
            return Ok(CourseEnrollmentApp_API);
        }

        [HttpDelete("{studentId}/{courseId:long}")]
        [Authorize]
        public async Task<ActionResult> RemoveCourseEnrollmentApp_API(string studentId, long courseId)
        {
            var CourseEnrollmentApp_API = await Context.StudentCourseEnrollmentApp_APIs.FindAsync(studentId, courseId);

            if (CourseEnrollmentApp_API is not null)
            {
                Context.StudentCourseEnrollmentApp_APIs.Remove(CourseEnrollmentApp_API);
                await Context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("{studentId}/{courseId:long?}")]
        [Authorize]
        public async Task<ActionResult> GetCourse(string studentId, long courseId)
        {
            if (courseId == 0)
            {
                var CourseEnrollmentApp_APIs = await Context.StudentCourseEnrollmentApp_APIs
                    .Where(se => se.StudentId == studentId)
                    .Include(se => se.Course) // Ensure you include the Course navigation property
                    .ToListAsync();
                var courses = CourseEnrollmentApp_APIs.Select(se => se.Course).ToList();
                return Ok(courses);
            }
            else if (courseId > 0 && !studentId.IsNullOrEmpty()) { }
            {
                var CourseEnrollmentApp_API = await Context.StudentCourseEnrollmentApp_APIs.FindAsync(studentId, courseId);
                return Ok(CourseEnrollmentApp_API);
            }
        }
    }
}
