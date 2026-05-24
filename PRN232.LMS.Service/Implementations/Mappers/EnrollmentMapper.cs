using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations.Mappers
{
    public static class EnrollmentMapper
    {
        public static EnrollmentBusiness MapToEnrollmentBusiness(Enrollment entity) =>
            entity == null ? null : new EnrollmentBusiness
            {
                EnrollmentId = entity.EnrollmentId,
                StudentId = entity.StudentId,
                CourseId = entity.CourseId,
                EnrollDate = entity.EnrollDate,
                Status = entity.Status,
                Student = StudentMapper.MapToStudentBusiness(entity.Student),
                Course = CourseMapper.MapToCourseBusiness(entity.Course)
            };

        public static Enrollment MapToEnrollment(EnrollmentBusiness business) =>
            business == null ? null : new Enrollment
            {
                EnrollmentId = business.EnrollmentId,
                StudentId = business.StudentId,
                CourseId = business.CourseId,
                EnrollDate = business.EnrollDate,
                Status = business.Status
            };
    }
}
