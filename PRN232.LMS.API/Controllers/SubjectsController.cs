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
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;

        public SubjectsController(ISubjectService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<List<SubjectResponse>>), 200)]
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

            var response = data.Select(b => new SubjectResponse
            {
                SubjectId = b.SubjectId,
                SubjectCode = b.SubjectCode,
                SubjectName = b.SubjectName,
                Credit = b.Credit
            }).ToList();

            return Ok(ResponseWrapper<List<SubjectResponse>>.SuccessResponse(response, "Subjects retrieved successfully", pagination));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string expand = null)
        {
            var business = await _service.GetByIdAsync(id, expand);
            if (business == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Subject with id {id} not found"));

            var response = new SubjectResponse
            {
                SubjectId = business.SubjectId,
                SubjectCode = business.SubjectCode,
                SubjectName = business.SubjectName,
                Credit = business.Credit
            };

            return Ok(ResponseWrapper<SubjectResponse>.SuccessResponse(response, "Subject retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<SubjectResponse>), 201)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new SubjectBusiness
            {
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            };

            var created = await _service.CreateAsync(business);
            var response = new SubjectResponse
            {
                SubjectId = created.SubjectId,
                SubjectCode = created.SubjectCode,
                SubjectName = created.SubjectName,
                Credit = created.Credit
            };

            return CreatedAtAction(nameof(GetById), new { id = created.SubjectId },
                ResponseWrapper<SubjectResponse>.SuccessResponse(response, "Subject created successfully"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new SubjectBusiness
            {
                SubjectId = id,
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            };

            var updated = await _service.UpdateAsync(id, business);
            if (updated == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Subject with id {id} not found"));

            var response = new SubjectResponse
            {
                SubjectId = updated.SubjectId,
                SubjectCode = updated.SubjectCode,
                SubjectName = updated.SubjectName,
                Credit = updated.Credit
            };

            return Ok(ResponseWrapper<SubjectResponse>.SuccessResponse(response, "Subject updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Subject with id {id} not found"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Subject deleted successfully"));
        }
    }
}
