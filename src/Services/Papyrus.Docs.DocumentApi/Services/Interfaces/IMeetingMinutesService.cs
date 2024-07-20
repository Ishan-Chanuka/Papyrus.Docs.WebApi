namespace Papyrus.Docs.DocumentApi.Services.Interfaces
{
    public interface IMeetingMinutesService
    {
        Task<ApiResponse<Guid>> CreateMeetingMinutesAsync(MeetingMinutesDto request);
        Task<ApiResponse<IEnumerable<MeetingMinutesResponseModel>>> GetMeetingMinutesAsync();
        Task<ApiResponse<Guid>> UpdateMeetingMinutesAsync(Guid id, MeetingMinutesDto request);
        Task<ApiResponse<Guid>> DeleteMeetingMinutesAsync(Guid id);
        Task<ApiResponse<MeetingMinutesResponseModel>> GetMeetingMinutesByIdAsync(Guid id);
    }
}
