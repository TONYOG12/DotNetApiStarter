using System.ComponentModel.DataAnnotations;
using DOMAIN.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DOMAIN.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity, IOrganizationType
{
    [PersonalData][StringLength(100)] public string FirstName { get; set; }
    [PersonalData][StringLength(100)] public string LastName { get; set; }
    [StringLength(100)] public string Title { get; set; }
    [StringLength(100)] public string Sex { get; set; }
    [StringLength(100)] public string EmployeeId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? LastUpdatedById { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? LastDeletedById { get; set; }
    public Guid? DirectReportId { get; set; }
    public User DirectReport { get; set; }
    public DateTime HiredOn { get; set; }
    [StringLength(100)] public string Avatar { get; set; }
    public bool IsDisabled { get; set; }
    [StringLength(100)] public string OrganizationName { get; set; }
}