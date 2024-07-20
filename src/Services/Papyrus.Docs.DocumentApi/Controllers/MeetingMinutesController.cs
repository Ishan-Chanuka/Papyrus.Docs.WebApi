using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Papyrus.Docs.DocumentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingMinutesController(IMeetingMinutesService _meetingMinuteService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<ApiResponse<string>>> CreateMeetingMinutesAsync(MeetingMinutesDto request)
        {
            var result = await _meetingMinuteService.CreateMeetingMinutesAsync(request);
            var url = $"{Request.GetDisplayUrl()}/{result.Data}";

            ApiResponse<string> response = new()
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                IsSuccess = result.IsSuccess,
                Data = url
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MeetingMinutesResponseModel>>>> GetMeetingMinutesAsync()
        {
            var response = await _meetingMinuteService.GetMeetingMinutesAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MeetingMinutesResponseModel>>> GetMeetingMinutesByIdAsync(Guid id)
        {
            var response = await _meetingMinuteService.GetMeetingMinutesByIdAsync(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateMeetingMinutesAsync(Guid id, MeetingMinutesDto request)
        {
            var response = await _meetingMinuteService.UpdateMeetingMinutesAsync(id, request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}/{userId}")]
        public async Task<ActionResult<ApiResponse<Guid>>> DeleteMeetingMinutesAsync(Guid id, Guid userId)
        {
            var response = await _meetingMinuteService.DeleteMeetingMinutesAsync(id, userId);

            return StatusCode(response.StatusCode, response);
        }
    }
}
