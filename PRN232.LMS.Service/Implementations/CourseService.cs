using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Service.Implementations.Mappers;
using PRN232.LMS.Service.Interfaces;
using PRN232.LMS.Service.Models;

namespace PRN232.LMS.Service.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _repository;

        public CourseService(IGenericRepository<Course> repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<CourseBusiness> data, int totalCount)> GetAllAsync(
            string search = null, string sort = null, int page = 1, int pageSize = 10, string expand = null)
        {
            var (data, totalCount) = await _repository.GetAllWithCountAsync(search, sort, page, pageSize, null, expand);
            return (data.Select(CourseMapper.MapToCourseBusiness), totalCount);
        }

        public async Task<CourseBusiness> GetByIdAsync(int id, string expand = null)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return CourseMapper.MapToCourseBusiness(entity);
        }

        public async Task<CourseBusiness> CreateAsync(CourseBusiness business)
        {
            var entity = CourseMapper.MapToCourse(business);
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return CourseMapper.MapToCourseBusiness(entity);
        }

        public async Task<CourseBusiness> UpdateAsync(int id, CourseBusiness business)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.CourseName = business.CourseName;
            entity.SemesterId = business.SemesterId;
            entity.SubjectId = business.SubjectId;

            await _repository.UpdateAsync(entity);
            await _repository.SaveAsync();
            return CourseMapper.MapToCourseBusiness(entity);
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
