using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Implementations.Mappers;
using PRN232.LMS.Service.Interfaces;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly IGenericRepository<Semester> _repository;

        public SemesterService(IGenericRepository<Semester> repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<SemesterBusiness> data, int totalCount)> GetAllAsync(
            string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null)
        {
            var (data, totalCount) = await _repository.GetAllWithCountAsync(search, sort, page, pageSize, null, expand);
            return (data.Select(SemesterMapper.MapToSemesterBusiness), totalCount);
        }

        public async Task<SemesterBusiness> GetByIdAsync(int id, string expand = null)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return SemesterMapper.MapToSemesterBusiness(entity);
        }

        public async Task<SemesterBusiness> CreateAsync(SemesterBusiness business)
        {
            var entity = SemesterMapper.MapToSemester(business);
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return SemesterMapper.MapToSemesterBusiness(entity);
        }

        public async Task<SemesterBusiness> UpdateAsync(int id, SemesterBusiness business)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.SemesterName = business.SemesterName;
            entity.StartDate = business.StartDate;
            entity.EndDate = business.EndDate;

            await _repository.UpdateAsync(entity);
            await _repository.SaveAsync();
            return SemesterMapper.MapToSemesterBusiness(entity);
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
