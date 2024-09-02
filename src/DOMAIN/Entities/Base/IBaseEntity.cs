namespace DOMAIN.Entities.Base;

public interface IBaseEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    Guid? CreatedById { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? LastUpdatedById { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? LastDeletedById { get; set; }
}