using CourseEnrollmentApp_API.Models;

namespace CourseEnrollmentApp_API.Services.TempProc
{
    public partial class TmpProc
    {
        const string ClassName = "Tmp2312";
        [TmpProcAtt(className: ClassName)]
        private async Task AddDefaultAdminUser()
        {
            await UserService.SeedRoles();
            await UserService.CreateAsync(new Controllers.Auth.RegisterModelIn
            {
                Email = Configuration.GetSection("DefaultUser")["Email"],
                Name = Configuration.GetSection("DefaultUser")["Email"],
                Password = Configuration.GetSection("DefaultUser")["Password"],
                Role = Roles.ADMIN
            });
        }

        [TmpProcAtt(className: ClassName)]
        private async Task GenerateAndSaveCourses()
        {
            var courses = new List<Course>{
    new Course {  Title = "Introduction to Computer Science", Description = "An introductory course on the basics of computer science." },
    new Course {  Title = "Programming Fundamentals", Description = "Learn the fundamentals of programming using C#." },
    new Course {  Title = "Object-Oriented Programming", Description = "Dive deep into the principles of object-oriented programming." },
    new Course { Title = "Data Structures and Algorithms", Description = "An in-depth look at data structures and algorithms." },
    new Course { Title = "Web Development", Description = "Learn the basics of web development, including HTML, CSS, and JavaScript." },
    new Course { Title = "Database Management Systems", Description = "An introduction to database concepts and SQL." },
    new Course {  Title = "Software Engineering", Description = "Principles and practices of software development." },
    new Course { Title = "Mobile Application Development", Description = "Developing mobile applications for Android and iOS." },
    new Course {  Title = "Cloud Computing", Description = "Introduction to cloud computing concepts and practices." },
    new Course {  Title = "Artificial Intelligence", Description = "Basics of artificial intelligence and machine learning." }
};
            foreach (var course in courses)
            {
                await Context.Courses.AddAsync(course);
            }
            await Context.SaveChangesAsync();
        }
    }
}
