namespace Papyrus.Docs.DocumentApi.Models.ResponseModels
{
    public class MeetingMinutesResponseModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? MeetingNo { get; set; }
        public string? Description { get; set; }
        public DateTime MeetingDate { get; set; }
    }
}
