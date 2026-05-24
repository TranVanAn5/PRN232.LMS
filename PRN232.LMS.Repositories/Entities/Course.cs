namespace PRN232.LMS.Repositories.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }

        public virtual Semester Semester { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
