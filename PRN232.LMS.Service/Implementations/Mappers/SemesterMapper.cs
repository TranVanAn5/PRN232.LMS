using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations.Mappers
{
    public static class SemesterMapper
    {
        public static SemesterBusiness MapToSemesterBusiness(Semester entity) =>
            entity == null ? null : new SemesterBusiness
            {
                SemesterId = entity.SemesterId,
                SemesterName = entity.SemesterName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };

        public static Semester MapToSemester(SemesterBusiness business) =>
            business == null ? null : new Semester
            {
                SemesterId = business.SemesterId,
                SemesterName = business.SemesterName,
                StartDate = business.StartDate,
                EndDate = business.EndDate
            };
    }
}
