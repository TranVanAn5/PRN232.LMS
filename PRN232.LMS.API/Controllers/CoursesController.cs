using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Service.Models;
using PRN232.LMS.Service.Interfaces;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _service;

        public CoursesController(ICourseService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<List<CourseResponse>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] string sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string expand = null)
        {
            var (data, totalCount) = await _service.GetAllAsync(search, sort, page, pageSize, expand);
            var pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (totalCount + pageSize - 1) / pageSize
            };

            var response = data.Select(b => new CourseResponse
            {
                CourseId = b.CourseId,
                CourseName = b.CourseName,
                SemesterId = b.SemesterId,
                SubjectId = b.SubjectId,
                Semester = b.Semester != null ? new SemesterResponse
                {
                    SemesterId = b.Semester.SemesterId,
                    SemesterName = b.Semester.SemesterName,
                    StartDate = b.Semester.StartDate,
                    EndDate = b.Semester.EndDate
                } : null,
                Subject = b.Subject != null ? new SubjectResponse
                {
                    SubjectId = b.Subject.SubjectId,
                    SubjectCode = b.Subject.SubjectCode,
                    SubjectName = b.Subject.SubjectName,
                    Credit = b.Subject.Credit
                } : null
            }).ToList();

            return Ok(ResponseWrapper<List<CourseResponse>>.SuccessResponse(response, "Courses retrieved successfully", pagination));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<CourseResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string expand = null)
        {
            var business = await _service.GetByIdAsync(id, expand);
            if (business == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Course with id {id} not found"));

            var response = new CourseResponse
            {
                CourseId = business.CourseId,
                CourseName = business.CourseName,
                SemesterId = business.SemesterId,
                SubjectId = business.SubjectId,
                Semester = business.Semester != null ? new SemesterResponse
                {
                    SemesterId = business.Semester.SemesterId,
                    SemesterName = business.Semester.SemesterName,
                    StartDate = business.Semester.StartDate,
                    EndDate = business.Semester.EndDate
                } : null,
                Subject = business.Subject != null ? new SubjectResponse
                {
                    SubjectId = business.Subject.SubjectId,
                    SubjectCode = business.Subject.SubjectCode,
                    SubjectName = business.Subject.SubjectName,
                    Credit = business.Subject.Credit
                } : null
            };

            return Ok(ResponseWrapper<CourseResponse>.SuccessResponse(response, "Course retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<CourseResponse>), 201)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new CourseBusiness
            {
                CourseName = request.CourseName,
                SemesterId = request.SemesterId,
                SubjectId = request.SubjectId
            };

            var created = await _service.CreateAsync(business);
            var response = new CourseResponse
            {
                CourseId = created.CourseId,
                CourseName = created.CourseName,
                SemesterId = created.SemesterId,
                SubjectId = created.SubjectId
            };

            return CreatedAtAction(nameof(GetById), new { id = created.CourseId },
                ResponseWrapper<CourseResponse>.SuccessResponse(response, "Course created successfully"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<CourseResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new CourseBusiness
            {
                CourseId = id,
                CourseName = request.CourseName,
                SemesterId = request.SemesterId,
                SubjectId = request.SubjectId
            };

            var updated = await _service.UpdateAsync(id, business);
            if (updated == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Course with id {id} not found"));

            var response = new CourseResponse
            {
                CourseId = updated.CourseId,
                CourseName = updated.CourseName,
                SemesterId = updated.SemesterId,
                SubjectId = updated.SubjectId
            };

            return Ok(ResponseWrapper<CourseResponse>.SuccessResponse(response, "Course updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Course with id {id} not found"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Course deleted successfully"));
        }
    }
}
