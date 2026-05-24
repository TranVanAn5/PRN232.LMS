namespace PRN232.LMS.API.Models.Requests
{
    public class CreateStudentRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UpdateStudentRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
