using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Interfaces
{
    public interface ISubjectService
    {
        Task<(IEnumerable<SubjectBusiness> data, int totalCount)> GetAllAsync(string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null);
        Task<SubjectBusiness> GetByIdAsync(int id, string expand = null);
        Task<SubjectBusiness> CreateAsync(SubjectBusiness business);
        Task<SubjectBusiness> UpdateAsync(int id, SubjectBusiness business);
        Task<bool> DeleteAsync(int id);
    }
}
