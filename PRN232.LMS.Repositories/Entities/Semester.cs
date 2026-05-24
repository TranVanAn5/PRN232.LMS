namespace PRN232.LMS.Repositories.Entities
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
