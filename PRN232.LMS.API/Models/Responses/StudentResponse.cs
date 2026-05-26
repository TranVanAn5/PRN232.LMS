namespace PRN232.LMS.API.Models.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
