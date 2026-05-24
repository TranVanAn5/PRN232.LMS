namespace PRN232.LMS.Service.Models
{
    public class CourseBusiness
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public SemesterBusiness Semester { get; set; }
        public SubjectBusiness Subject { get; set; }
    }
}
