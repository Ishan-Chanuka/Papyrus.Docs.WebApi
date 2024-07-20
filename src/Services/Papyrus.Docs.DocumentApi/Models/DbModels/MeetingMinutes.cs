using Papyrus.Docs.DocumentApi.Primitives.BaseEntity;

namespace Papyrus.Docs.DocumentApi.Models.DbModels
{
    public class MeetingMinutes : BaseEntity
    {
        public required string Title { get; set; }
        public required string MeetingNo { get; set; }
        public string? Description { get; set; }
        public DateTime MeetingDate { get; set; }
    }
}
