using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Interfaces
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseBusiness> data, int totalCount)> GetAllAsync(string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null);
        Task<CourseBusiness> GetByIdAsync(int id, string expand = null);
        Task<CourseBusiness> CreateAsync(CourseBusiness business);
        Task<CourseBusiness> UpdateAsync(int id, CourseBusiness business);
        Task<bool> DeleteAsync(int id);
    }
}
