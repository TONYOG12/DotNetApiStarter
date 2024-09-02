using System.ComponentModel.DataAnnotations;
using DOMAIN.Entities.Roles;
using Microsoft.AspNetCore.Identity;

namespace DOMAIN.Entities.Users;

public class UserDto
{
    public Guid Id { get; set; }
    [PersonalData] public string FirstName { get; set; }
    [PersonalData] public string LastName { get; set; }
    public string EmployeeId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; }
    [EmailAddress] public string Email { get; set; }
    [EmailAddress] public string PrivateEmail { get; set; }
    [Phone] public string PhoneNumber { get; set; }
    public string Title { get; set; }
    public string Sex { get; set; }
    public DateTime HiredOn { get; set; }
    public string Avatar { get; set; }
    public bool IsDisabled { get; set; }
    public ICollection<RoleDto> Roles { get; set; }
}