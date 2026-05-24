using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Interfaces
{
    public interface IEnrollmentService
    {
        Task<(IEnumerable<EnrollmentBusiness> data, int totalCount)> GetAllAsync(string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null);
        Task<EnrollmentBusiness> GetByIdAsync(int id, string expand = null);
        Task<EnrollmentBusiness> CreateAsync(EnrollmentBusiness business);
        Task<EnrollmentBusiness> UpdateAsync(int id, EnrollmentBusiness business);
        Task<bool> DeleteAsync(int id);
    }
}
