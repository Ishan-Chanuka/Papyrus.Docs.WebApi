using System.ComponentModel.DataAnnotations;

namespace Papyrus.Docs.DocumentApi.Primitives.BaseEntity
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Guid CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedUser { get; set; }
        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
