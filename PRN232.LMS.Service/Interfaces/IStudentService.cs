using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Interfaces
{
    public interface IStudentService
    {
        Task<(IEnumerable<StudentBusiness> data, int totalCount)> GetAllAsync(string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null);
        Task<StudentBusiness> GetByIdAsync(int id, string expand = null);
        Task<StudentBusiness> CreateAsync(StudentBusiness business);
        Task<StudentBusiness> UpdateAsync(int id, StudentBusiness business);
        Task<bool> DeleteAsync(int id);
    }
}
