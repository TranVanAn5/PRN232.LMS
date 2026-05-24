using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Implementations.Mappers;
using PRN232.LMS.Service.Interfaces;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _repository;

        public EnrollmentService(IGenericRepository<Enrollment> repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<EnrollmentBusiness> data, int totalCount)> GetAllAsync(
            string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null)
        {
            var (data, totalCount) = await _repository.GetAllWithCountAsync(search, sort, page, pageSize, null, expand);
            return (data.Select(EnrollmentMapper.MapToEnrollmentBusiness), totalCount);
        }

        public async Task<EnrollmentBusiness> GetByIdAsync(int id, string expand = null)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return EnrollmentMapper.MapToEnrollmentBusiness(entity);
        }

        public async Task<EnrollmentBusiness> CreateAsync(EnrollmentBusiness business)
        {
            var entity = EnrollmentMapper.MapToEnrollment(business);
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return EnrollmentMapper.MapToEnrollmentBusiness(entity);
        }

        public async Task<EnrollmentBusiness> UpdateAsync(int id, EnrollmentBusiness business)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.StudentId = business.StudentId;
            entity.CourseId = business.CourseId;
            entity.EnrollDate = business.EnrollDate;
            entity.Status = business.Status;

            await _repository.UpdateAsync(entity);
            await _repository.SaveAsync();
            return EnrollmentMapper.MapToEnrollmentBusiness(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
                await _repository.SaveAsync();
            return deleted;
        }
    }
}
