using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Interfaces
{
    public interface ISemesterService
    {
        Task<(IEnumerable<SemesterBusiness> data, int totalCount)> GetAllAsync(string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null);
        Task<SemesterBusiness> GetByIdAsync(int id, string expand = null);
        Task<SemesterBusiness> CreateAsync(SemesterBusiness business);
        Task<SemesterBusiness> UpdateAsync(int id, SemesterBusiness business);
        Task<bool> DeleteAsync(int id);
    }
}
