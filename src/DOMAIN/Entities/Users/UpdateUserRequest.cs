using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DOMAIN.Entities.Users;

public class UpdateUserRequest 
{
    [Required][PersonalData] public string FirstName { get; set; }
    [Required][PersonalData] public string LastName { get; set; }
    public string EmployeeId { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    [EmailAddress] public string PrivateEmail { get; set; }
    [Required] [Phone] public string PhoneNumber { get; set; }
    public string Title { get; set; }
    public string Sex { get; set; }
    public Guid? DirectReportId { get; set; }
    public Guid? EmploymentTypeId { get; set; }
    [Required]public DateTime HiredOn { get; set; }
    public IEnumerable<string> RoleNames { get; set; }
    public string Password { get; set; }
    public string Avatar { get; set; }
}