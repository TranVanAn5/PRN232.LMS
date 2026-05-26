using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations.Mappers
{
    public static class StudentMapper
    {
        public static StudentBusiness MapToStudentBusiness(Student entity) =>
            entity == null ? null : new StudentBusiness
            {
                StudentId = entity.StudentId,
                FullName = entity.FullName,
                Email = entity.Email,
                DateOfBirth = entity.DateOfBirth
            };

        public static Student MapToStudent(StudentBusiness business) =>
            business == null ? null : new Student
            {
                StudentId = business.StudentId,
                FullName = business.FullName,
                Email = business.Email,
                DateOfBirth = business.DateOfBirth
            };
    }
}
