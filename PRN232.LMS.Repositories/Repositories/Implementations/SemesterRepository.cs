using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Implementations
{
    public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
    {
        public SemesterRepository(LmsDbContext context) : base(context) { }
    }
}
