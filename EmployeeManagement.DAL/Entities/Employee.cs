using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.DAL;

public class Employee
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public string NationalId { get; set; }

    [Required]
    [Range(18, 100)]
    public int Age { get; set; }

    public string? SignatureUrl { get; set; } 

    public string UserId { get; set; } // Link to AspNetUsers

    public virtual ICollection<Attendance>? Attendances { get; set; } = new HashSet<Attendance>();

}
