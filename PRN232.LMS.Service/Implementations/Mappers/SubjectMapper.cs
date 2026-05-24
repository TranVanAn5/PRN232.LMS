using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations.Mappers
{
    public static class SubjectMapper
    {
        public static SubjectBusiness MapToSubjectBusiness(Subject entity) =>
            entity == null ? null : new SubjectBusiness
            {
                SubjectId = entity.SubjectId,
                SubjectCode = entity.SubjectCode,
                SubjectName = entity.SubjectName,
                Credit = entity.Credit
            };

        public static Subject MapToSubject(SubjectBusiness business) =>
            business == null ? null : new Subject
            {
                SubjectId = business.SubjectId,
                SubjectCode = business.SubjectCode,
                SubjectName = business.SubjectName,
                Credit = business.Credit
            };
    }
}
