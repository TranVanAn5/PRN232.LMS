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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all students with pagination, search, sort, and expand support
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<List<StudentResponse>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] string sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string fields = null,
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

            var response = data.Select(b => new StudentResponse
            {
                StudentId = b.StudentId,
                FullName = b.FullName,
                Email = b.Email,
                DateOfBirth = b.DateOfBirth,
                Enrollments = b.Enrollments?.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList() ?? new()
            }).ToList();

            return Ok(ResponseWrapper<List<StudentResponse>>.SuccessResponse(response, "Students retrieved successfully", pagination));
        }

        /// <summary>
        /// Get student by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<StudentResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string expand = null)
        {
            var business = await _service.GetByIdAsync(id, expand);
            if (business == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Student with id {id} not found"));

            var response = new StudentResponse
            {
                StudentId = business.StudentId,
                FullName = business.FullName,
                Email = business.Email,
                DateOfBirth = business.DateOfBirth,
                Enrollments = business.Enrollments?.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList() ?? new()
            };

            return Ok(ResponseWrapper<StudentResponse>.SuccessResponse(response, "Student retrieved successfully"));
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<StudentResponse>), 201)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new StudentBusiness
            {
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };

            var created = await _service.CreateAsync(business);
            var response = new StudentResponse
            {
                StudentId = created.StudentId,
                FullName = created.FullName,
                Email = created.Email,
                DateOfBirth = created.DateOfBirth
            };

            return CreatedAtAction(nameof(GetById), new { id = created.StudentId },
                ResponseWrapper<StudentResponse>.SuccessResponse(response, "Student created successfully"));
        }

        /// <summary>
        /// Update a student
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<StudentResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new StudentBusiness
            {
                StudentId = id,
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };

            var updated = await _service.UpdateAsync(id, business);
            if (updated == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Student with id {id} not found"));

            var response = new StudentResponse
            {
                StudentId = updated.StudentId,
                FullName = updated.FullName,
                Email = updated.Email,
                DateOfBirth = updated.DateOfBirth
            };

            return Ok(ResponseWrapper<StudentResponse>.SuccessResponse(response, "Student updated successfully"));
        }

        /// <summary>
        /// Delete a student
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Student with id {id} not found"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Student deleted successfully"));
        }
    }
}
