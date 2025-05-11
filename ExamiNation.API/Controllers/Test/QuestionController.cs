using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.DTOs.RequestParams;
using ExamiNation.Application.Interfaces.Test;
using ExamiNation.Application.Services.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IMapper _mapper;

        public QuestionController(IQuestionService questionService, IMapper mapper)
        {
            _questionService = questionService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var response = await _questionService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Question ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }
            var response = await _questionService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetQuestionsByTest(Guid testId)
        {
            var result = await _questionService.GetByTestIdAsync(testId);
            return Ok(result);
        }

        [HttpGet("get-pages")]
        public async Task<IActionResult> GetPagedTests([FromQuery] QueryParameters queryParameters)
        {
            var response = await _questionService.GetAllQuestionWithOptionsPagedAsync(queryParameters);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(response);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
        {
            if (createQuestionDto == null)
                return BadRequest("Question data cannot be null.");

            var response = await _questionService.AddAsync(createQuestionDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetQuestionById), new { id = response.Data.Id }, response.Data);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] EditQuestionDto editQuestionDto)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Question ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            if (editQuestionDto == null)
            {
                return BadRequest("Question data cannot be null.");
            }
            if (editQuestionDto.Id != id)
            {
                return BadRequest("Question ID in the request body does not match the ID in the URL.");
            }

            editQuestionDto.Id = id;
            var response = await _questionService.UpdateAsync(editQuestionDto);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<OptionDto>.CreateErrorResponse("Question ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _questionService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }


    }

}