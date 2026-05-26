using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories.Implementations
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(LmsDbContext context) : base(context) { }
    }
}
