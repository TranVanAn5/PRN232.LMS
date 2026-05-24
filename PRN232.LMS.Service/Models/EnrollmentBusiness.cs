namespace PRN232.LMS.Service.Models
{
    public class EnrollmentBusiness
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; }
        public StudentBusiness Student { get; set; }
        public CourseBusiness Course { get; set; }
    }
}
