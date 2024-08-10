using Microsoft.EntityFrameworkCore;
using sms.backend.Models;

namespace sms.backend.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Staff?> Staff { get; set; }
        public DbSet<Class?> Classes { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<TeacherEnrollment> TeacherEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.ClassId });
            // Define other relationships and keys as needed
            modelBuilder.Entity<TeacherEnrollments>()
                .HasKey(e => new { e.StaffId, e.ClassId });
            // Define other relationships and keys as needed
        }
    }
}