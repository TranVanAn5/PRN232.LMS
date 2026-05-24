using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Implementations.Mappers;
using PRN232.LMS.Service.Interfaces;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _repository;

        public SubjectService(IGenericRepository<Subject> repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<SubjectBusiness> data, int totalCount)> GetAllAsync(
            string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null)
        {
            var (data, totalCount) = await _repository.GetAllWithCountAsync(search, sort, page, pageSize, null, expand);
            return (data.Select(SubjectMapper.MapToSubjectBusiness), totalCount);
        }

        public async Task<SubjectBusiness> GetByIdAsync(int id, string expand = null)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return SubjectMapper.MapToSubjectBusiness(entity);
        }

        public async Task<SubjectBusiness> CreateAsync(SubjectBusiness business)
        {
            var entity = SubjectMapper.MapToSubject(business);
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return SubjectMapper.MapToSubjectBusiness(entity);
        }

        public async Task<SubjectBusiness> UpdateAsync(int id, SubjectBusiness business)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.SubjectCode = business.SubjectCode;
            entity.SubjectName = business.SubjectName;
            entity.Credit = business.Credit;

            await _repository.UpdateAsync(entity);
            await _repository.SaveAsync();
            return SubjectMapper.MapToSubjectBusiness(entity);
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
