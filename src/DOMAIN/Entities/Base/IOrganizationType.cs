using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Base;

public interface IOrganizationType
{
    [StringLength(100)] string OrganizationName { get; set; }
}