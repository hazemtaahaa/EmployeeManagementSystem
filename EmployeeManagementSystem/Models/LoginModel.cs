using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API;

public class LoginModel
{
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
