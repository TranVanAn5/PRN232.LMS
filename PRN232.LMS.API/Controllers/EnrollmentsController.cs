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
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentsController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<List<EnrollmentResponse>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] string sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string expand = null)
        {
            expand = string.IsNullOrWhiteSpace(expand) ? "Student,Course" : expand;
            var (data, totalCount) = await _service.GetAllAsync(search, sort, page, pageSize, expand);
            var pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (totalCount + pageSize - 1) / pageSize
            };

            var response = data.Select(b => MapToResponse(b)).ToList();
            return Ok(ResponseWrapper<List<EnrollmentResponse>>.SuccessResponse(response, "Enrollments retrieved successfully", pagination));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<EnrollmentResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string expand = null)
        {
            expand = string.IsNullOrWhiteSpace(expand) ? "Student,Course" : expand;
            var business = await _service.GetByIdAsync(id, expand);
            if (business == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Enrollment with id {id} not found"));

            var response = MapToResponse(business);
            return Ok(ResponseWrapper<EnrollmentResponse>.SuccessResponse(response, "Enrollment retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<EnrollmentResponse>), 201)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new EnrollmentBusiness
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            var created = await _service.CreateAsync(business);
            var response = MapToResponse(created);

            return CreatedAtAction(nameof(GetById), new { id = created.EnrollmentId },
                ResponseWrapper<EnrollmentResponse>.SuccessResponse(response, "Enrollment created successfully"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<EnrollmentResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new EnrollmentBusiness
            {
                EnrollmentId = id,
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            var updated = await _service.UpdateAsync(id, business);
            if (updated == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Enrollment with id {id} not found"));

            var response = MapToResponse(updated);
            return Ok(ResponseWrapper<EnrollmentResponse>.SuccessResponse(response, "Enrollment updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Enrollment with id {id} not found"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Enrollment deleted successfully"));
        }

        private EnrollmentResponse MapToResponse(EnrollmentBusiness business)
        {
            return new EnrollmentResponse
            {
                EnrollmentId = business.EnrollmentId,
                StudentId = business.StudentId,
                CourseId = business.CourseId,
                EnrollDate = business.EnrollDate,
                Status = business.Status,
                Student = business.Student != null ? new StudentResponse
                {
                    StudentId = business.Student.StudentId,
                    FullName = business.Student.FullName,
                    Email = business.Student.Email,
                    DateOfBirth = business.Student.DateOfBirth
                } : null,
                Course = business.Course != null ? new CourseResponse
                {
                    CourseId = business.Course.CourseId,
                    CourseName = business.Course.CourseName,
                    SemesterId = business.Course.SemesterId,
                    SubjectId = business.Course.SubjectId,
                    Semester = business.Course.Semester != null ? new SemesterResponse
                    {
                        SemesterId = business.Course.Semester.SemesterId,
                        SemesterName = business.Course.Semester.SemesterName,
                        StartDate = business.Course.Semester.StartDate,
                        EndDate = business.Course.Semester.EndDate
                    } : null,
                    Subject = business.Course.Subject != null ? new SubjectResponse
                    {
                        SubjectId = business.Course.Subject.SubjectId,
                        SubjectCode = business.Course.Subject.SubjectCode,
                        SubjectName = business.Course.Subject.SubjectName,
                        Credit = business.Course.Subject.Credit
                    } : null
                } : null
            };
        }
    }
}
