using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API;

public class RegisterModel 
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; } // "Admin" or "Employee"

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalId { get; set; }
    public int Age { get; set; }



}
