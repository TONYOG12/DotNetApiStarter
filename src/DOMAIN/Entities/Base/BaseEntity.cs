using DOMAIN.Entities.Users;

namespace DOMAIN.Entities.Base;

public class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public Guid? LastUpdatedById { get; set; }
    public User LastUpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? LastDeletedById { get; set; }
    public User LastDeletedBy { get; set; }
}