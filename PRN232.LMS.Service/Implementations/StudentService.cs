using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Implementations.Mappers;
using PRN232.LMS.Service.Interfaces;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _repository;

        public StudentService(IGenericRepository<Student> repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<StudentBusiness> data, int totalCount)> GetAllAsync(
            string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null)
        {
            var (data, totalCount) = await _repository.GetAllWithCountAsync(search, sort, page, pageSize, null, expand);
            return (data.Select(StudentMapper.MapToStudentBusiness), totalCount);
        }

        public async Task<StudentBusiness> GetByIdAsync(int id, string expand = null)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return StudentMapper.MapToStudentBusiness(entity);
        }

        public async Task<StudentBusiness> CreateAsync(StudentBusiness business)
        {
            var entity = StudentMapper.MapToStudent(business);
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return StudentMapper.MapToStudentBusiness(entity);
        }

        public async Task<StudentBusiness> UpdateAsync(int id, StudentBusiness business)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.FullName = business.FullName;
            entity.Email = business.Email;
            entity.DateOfBirth = business.DateOfBirth;

            await _repository.UpdateAsync(entity);
            await _repository.SaveAsync();
            return StudentMapper.MapToStudentBusiness(entity);
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
