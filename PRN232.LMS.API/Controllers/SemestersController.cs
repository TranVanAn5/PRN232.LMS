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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _service;

        public SemestersController(ISemesterService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<List<SemesterResponse>>), 200)]
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

            var response = data.Select(b => new SemesterResponse
            {
                SemesterId = b.SemesterId,
                SemesterName = b.SemesterName,
                StartDate = b.StartDate,
                EndDate = b.EndDate
            }).ToList();

            return Ok(ResponseWrapper<List<SemesterResponse>>.SuccessResponse(response, "Semesters retrieved successfully", pagination));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<SemesterResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> GetById(int id, [FromQuery] string expand = null)
        {
            var business = await _service.GetByIdAsync(id, expand);
            if (business == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Semester with id {id} not found"));

            var response = new SemesterResponse
            {
                SemesterId = business.SemesterId,
                SemesterName = business.SemesterName,
                StartDate = business.StartDate,
                EndDate = business.EndDate
            };

            return Ok(ResponseWrapper<SemesterResponse>.SuccessResponse(response, "Semester retrieved successfully"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<SemesterResponse>), 201)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 400)]
        public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new SemesterBusiness
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var created = await _service.CreateAsync(business);
            var response = new SemesterResponse
            {
                SemesterId = created.SemesterId,
                SemesterName = created.SemesterName,
                StartDate = created.StartDate,
                EndDate = created.EndDate
            };

            return CreatedAtAction(nameof(GetById), new { id = created.SemesterId },
                ResponseWrapper<SemesterResponse>.SuccessResponse(response, "Semester created successfully"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<SemesterResponse>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseWrapper<object>.FailureResponse("Invalid request", ModelState.Values.SelectMany(v => v.Errors)));

            var business = new SemesterBusiness
            {
                SemesterId = id,
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var updated = await _service.UpdateAsync(id, business);
            if (updated == null)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Semester with id {id} not found"));

            var response = new SemesterResponse
            {
                SemesterId = updated.SemesterId,
                SemesterName = updated.SemesterName,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate
            };

            return Ok(ResponseWrapper<SemesterResponse>.SuccessResponse(response, "Semester updated successfully"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ResponseWrapper<object>.NotFoundResponse($"Semester with id {id} not found"));

            return Ok(ResponseWrapper<object>.SuccessResponse(null, "Semester deleted successfully"));
        }
    }
}
