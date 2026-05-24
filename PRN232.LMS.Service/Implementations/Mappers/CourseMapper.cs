using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations.Mappers
{
    public static class CourseMapper
    {
        public static CourseBusiness MapToCourseBusiness(Course entity) =>
            entity == null ? null : new CourseBusiness
            {
                CourseId = entity.CourseId,
                CourseName = entity.CourseName,
                SemesterId = entity.SemesterId,
                SubjectId = entity.SubjectId,
                Semester = SemesterMapper.MapToSemesterBusiness(entity.Semester),
                Subject = SubjectMapper.MapToSubjectBusiness(entity.Subject)
            };

        public static Course MapToCourse(CourseBusiness business) =>
            business == null ? null : new Course
            {
                CourseId = business.CourseId,
                CourseName = business.CourseName,
                SemesterId = business.SemesterId,
                SubjectId = business.SubjectId
            };
    }
}
