using System.ComponentModel.DataAnnotations;
using DOMAIN.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DOMAIN.Entities.Roles;

public class Role : IdentityRole<Guid>, IBaseEntity
{
    [StringLength(100)] public string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? LastUpdatedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? LastDeletedById { get; set; }
}