using AutoMapper;
using ExamiNation.Application.Common.Autorization;
using ExamiNation.Application.DTOs.ApiResponse;
using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.Interfaces.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamiNation.API.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly IMapper _mapper;

        public AnswerController(IAnswerService answerService, IMapper mapper)
        {
            _answerService = answerService;
            _mapper = mapper;
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet]
        public async Task<IActionResult> GetAllAnswers()
        {
            var response = await _answerService.GetAllAsync();

            if (!response.Success)
                return NotFound(response.Message);


            return Ok(response);
        }

        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(Guid id)
        {
            if (id == Guid.Empty)
            {
                var errorResponse = ApiResponse<AnswerDto>.CreateErrorResponse("Answer ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _answerService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }


        [Authorize(Roles = RoleGroups.AdminOrDevOrCreator)]
        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetAnsweresByTestId(Guid testId)
        {
            if (testId == Guid.Empty)
            {
                var errorResponse = ApiResponse<AnswerDto>.CreateErrorResponse("Test ID must be a valid non-empty GUID.");
                return BadRequest(errorResponse.Message);
            }

            var response = await _answerService.GetAllByTestAsync(testId);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

    }

}