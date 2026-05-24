using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Semester configuration
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId);
                entity.Property(e => e.SemesterName).HasMaxLength(100).IsRequired();
            });

            // Subject configuration
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.Property(e => e.SubjectCode).HasMaxLength(20).IsRequired();
                entity.Property(e => e.SubjectName).HasMaxLength(100).IsRequired();
            });

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseName).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.Semester)
                    .WithMany(s => s.Courses)
                    .HasForeignKey(e => e.SemesterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Subject)
                    .WithMany(s => s.Courses)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            });

            // Enrollment configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Semesters (6 semesters)
            var semesters = new List<Semester>
            {
                new() { SemesterId = 1, SemesterName = "Spring 2023", StartDate = new DateTime(2023, 1, 15), EndDate = new DateTime(2023, 5, 30) },
                new() { SemesterId = 2, SemesterName = "Summer 2023", StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023, 8, 31) },
                new() { SemesterId = 3, SemesterName = "Fall 2023", StartDate = new DateTime(2023, 9, 1), EndDate = new DateTime(2023, 12, 15) },
                new() { SemesterId = 4, SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 30) },
                new() { SemesterId = 5, SemesterName = "Summer 2024", StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 8, 31) },
                new() { SemesterId = 6, SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 15) }
            };
            modelBuilder.Entity<Semester>().HasData(semesters);

            // Seed Subjects (10 subjects)
            var subjects = new List<Subject>
            {
                new() { SubjectId = 1, SubjectCode = "CS101", SubjectName = "Introduction to Computer Science", Credit = 3 },
                new() { SubjectId = 2, SubjectCode = "CS201", SubjectName = "Data Structures", Credit = 3 },
                new() { SubjectId = 3, SubjectCode = "CS301", SubjectName = "Algorithms", Credit = 3 },
                new() { SubjectId = 4, SubjectCode = "CS401", SubjectName = "Database Systems", Credit = 4 },
                new() { SubjectId = 5, SubjectCode = "CS501", SubjectName = "Software Engineering", Credit = 4 },
                new() { SubjectId = 6, SubjectCode = "MATH101", SubjectName = "Calculus I", Credit = 4 },
                new() { SubjectId = 7, SubjectCode = "MATH201", SubjectName = "Linear Algebra", Credit = 3 },
                new() { SubjectId = 8, SubjectCode = "PHYS101", SubjectName = "Physics I", Credit = 4 },
                new() { SubjectId = 9, SubjectCode = "ENG101", SubjectName = "English Composition", Credit = 3 },
                new() { SubjectId = 10, SubjectCode = "HIST101", SubjectName = "World History", Credit = 3 }
            };
            modelBuilder.Entity<Subject>().HasData(subjects);

            // Seed Courses (20 courses across semesters)
            var courses = new List<Course>();
            int courseId = 1;
            for (int s = 1; s <= 3; s++)
            {
                for (int subj = 1; subj <= 7; subj++)
                {
                    courses.Add(new Course
                    {
                        CourseId = courseId,
                        CourseName = $"{subjects[subj - 1].SubjectName} - Semester {s}",
                        SemesterId = s,
                        SubjectId = subj
                    });
                    courseId++;
                    if (courseId > 20) break;
                }
                if (courseId > 20) break;
            }
            modelBuilder.Entity<Course>().HasData(courses);

            // Seed Students (50 students)
            var students = new List<Student>();
            var firstNames = new[] { "Nguyen", "Tran", "Pham", "Hoang", "Vu", "Le", "Bui", "Do", "Dang", "Ly" };
            var lastNames = new[] { "An", "Binh", "Chi", "Duc", "Em", "Phuong", "Giang", "Hung", "Ivy", "Khoa" };
            int studentId = 1;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    students.Add(new Student
                    {
                        StudentId = studentId,
                        FullName = $"{firstNames[j]} {lastNames[i]}",
                        Email = $"student{studentId:D3}@university.edu",
                        DateOfBirth = new DateTime(2000 + i, (j % 12) + 1, (j % 28) + 1)
                    });
                    studentId++;
                }
            }
            modelBuilder.Entity<Student>().HasData(students);

            // Seed Enrollments (500 enrollments)
            var enrollments = new List<Enrollment>();
            int enrollmentId = 1;
            var random = new Random(42);
            var statuses = new[] { "Active", "Completed", "Dropped", "In Progress" };

            for (int studentIdx = 1; studentIdx <= 50; studentIdx++)
            {
                int enrollmentsPerStudent = random.Next(8, 15);
                for (int e = 0; e < enrollmentsPerStudent; e++)
                {
                    int courseIdx = random.Next(1, courses.Count + 1);
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = enrollmentId,
                        StudentId = studentIdx,
                        CourseId = courseIdx,
                        EnrollDate = new DateTime(2023, random.Next(1, 13), random.Next(1, 29)),
                        Status = statuses[random.Next(statuses.Length)]
                    });
                    enrollmentId++;
                    if (enrollmentId > 500) break;
                }
                if (enrollmentId > 500) break;
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}
