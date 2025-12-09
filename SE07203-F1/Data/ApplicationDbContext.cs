using Microsoft.EntityFrameworkCore;
using SE07203_F1.Models;

namespace SE07203_F1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Danh sách các Bảng (DbSet) ---
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ClassSection> ClassSections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        // --- Cấu hình mối quan hệ ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình khóa ngoại cho Accounts
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .IsRequired();

            // 2. Cấu hình khóa ngoại cho Lecturers
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.Account)
                .WithOne() // 1 Account có 1 Lecturer
                .HasForeignKey<Lecturer>(l => l.AccountId)
                .IsRequired();

            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.Department)
                .WithMany(d => d.Lecturers)
                .HasForeignKey(l => l.DepartmentId)
                .IsRequired();

            // 3. Cấu hình khóa ngoại cho Students
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne() // 1 Account có 1 Student
                .HasForeignKey<Student>(s => s.AccountId)
                .IsRequired();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .IsRequired();

            // 4. Cấu hình mối quan hệ N-N (Enrollment)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .IsRequired();

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ClassSection)
                .WithMany(cs => cs.Enrollments)
                .HasForeignKey(e => e.ClassSectionId)
                .IsRequired();

            // 5. Cấu hình khóa ngoại cho ClassSections
            modelBuilder.Entity<ClassSection>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.ClassSections)
                .HasForeignKey(cs => cs.CourseId)
                .IsRequired();

            modelBuilder.Entity<ClassSection>()
                .HasOne(cs => cs.Lecturer)
                .WithMany(l => l.ClassSections)
                .HasForeignKey(cs => cs.LecturerId)
                .IsRequired();
        }
    }
}
