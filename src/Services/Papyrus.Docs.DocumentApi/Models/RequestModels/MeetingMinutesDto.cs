namespace Papyrus.Docs.DocumentApi.Models.RequestModels
{
    public record MeetingMinutesDto(string Title, string MeetingNo, string? Description, DateTime MeetingDate, Guid CreatedUser);
}