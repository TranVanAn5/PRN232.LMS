namespace PRN232.LMS.Repositories.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credit { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
