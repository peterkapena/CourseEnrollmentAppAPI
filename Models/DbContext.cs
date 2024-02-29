using CourseEnrollmentApp_API.Models.TmpProc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static CourseEnrollmentApp_API.Models.Pub;

namespace CourseEnrollmentApp_API.Models
{
    public class DBContext(DbContextOptions<DBContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<ErrorLog> Errors { get; set; }
        public DbSet<TmpTask> TmpTasks { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourseEnrollmentApp_API> StudentCourseEnrollmentApp_APIs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourseEnrollmentApp_API>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<StudentCourseEnrollmentApp_API>()
                .HasOne(e => e.Student)
                .WithMany(s => s.StudentCourseEnrollmentApp_APIs)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<StudentCourseEnrollmentApp_API>()
                .HasOne(e => e.Course)
                .WithMany(c => c.StudentCourseEnrollmentApp_APIs)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.StudentId);
        }
    }
}
